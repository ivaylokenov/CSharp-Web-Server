namespace MyWebServer.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using MyWebServer.Http;
    using MyWebServer.Routing;
    using System.Text;

    public static class RoutingTableExtensions
    {
        private const string Backslash = "/";
        private static Type stringType = typeof(string);
        private static Type httpResponseType = typeof(HttpResponse);
		
        public static IRoutingTable MapGet<TController>(
            this IRoutingTable routingTable,
            string path,
            Func<TController, HttpResponse> controllerFunction)
            where TController : Controller
            => routingTable.MapGet(path, request => controllerFunction(
                CreateController<TController>(request)));

        /// <summary>
        /// Maps all controllers found in the entry assembly to the given routing table.
        /// </summary>
        /// <param name="routingTable">Routing table to which the controllers are to be attached.</param>
        /// <returns>Routing table should more actions need to be appended to it.</returns>
        public static IRoutingTable MapControllers(
            this IRoutingTable routingTable)
        {
            //1. I believe the entry assembly to be the most abstract solution here.
            //2. GetCallingAssembly works as well, but it seems a bit more specific to me.
            //A little more prone to bugs perhaps.
            //3. I can see the possibility of the entry assembly having some issues as well.
            //I assume this can happen if the first executed assembly is not the one in the server,
            //but it has to be tested. For now, it's working.
            var assembly = Assembly.GetEntryAssembly();

            //Checks for name ending with controller and inheritance from base Controller type.
            var controllerTypes = assembly.GetTypes()
                .Where(t => !t.IsAbstract
                    && t.IsAssignableTo(typeof(Controller))
                    && t.Name.EndsWith(nameof(Controller)));

            foreach (var controller in controllerTypes)
            {
                var actions = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m.ReturnType.IsAssignableTo(httpResponseType));

                foreach (var action in actions)
                {

                    var httpMethod = HttpMethod.Get;

                    var httpMethodAttribute = action
                        .GetCustomAttribute<HttpMethodAttribute>();

                    if (httpMethodAttribute != null)
                    {
                        httpMethod = httpMethodAttribute.HttpMethod;
                    }

                    var path = GetRoutePath(controller.Name, action.Name);

                    var responseFunction = GetResponseFunction(action);

                    routingTable.Map(httpMethod, path, responseFunction);

                    MapDefaultRoutes(
                        routingTable,
                        httpMethod,
                        controller.Name,
                        action.Name,
                        responseFunction);
                }

            }

            return routingTable;
        }

        /// <summary>
        /// Gets the proper path for the route, given controller and action strings.
        /// </summary>
        /// <param name="controller">The name of the controller.</param>
        /// <param name="action"></param>
        /// <returns></returns>
        private static string GetRoutePath(string controller, string action)
        {
            controller = controller.Substring(0, controller.LastIndexOf("Controller"));

            StringBuilder path = new StringBuilder(Backslash);

            if(controller != "Home")
            {
                path.Append(controller).Append(Backslash);
            }

            if (action != "Index")
            {
                path.Append(action);
            }

            return path.ToString();
        }

        /// <summary>
        /// Maps each action in a controller to the routing table using reflection.
        /// </summary>
        /// <typeparam name="TController">The controller whose actions will be binded.</typeparam>
        /// <param name="routingTable">Routing table instance to which calls will be bound.</param>
        /// <returns>Routing table so that calls can be chained.</returns>
        public static IRoutingTable MapGet<TController>(
            this IRoutingTable routingTable)
            where TController : Controller
        {
            var controllerActions = typeof(TController)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => m.GetParameters().Length == 0);

            foreach (var action in controllerActions)
            {
                //Separates actual controller name from the controller classes.
                var controller = typeof(TController);

                var controllerName = controller.Name.Substring(0, controller.Name.LastIndexOf("Controller"));

                //The home controller has an empty route; all others have their name
                var path = new StringBuilder(controllerName.ToLower() == "home" ?
                    string.Empty : Backslash + controllerName);

                path.Append((action.Name.ToLower() == "index" || action.Name == string.Empty) 
                    ? Backslash : Backslash + action.Name);

                routingTable.MapGet(path.ToString(), request =>
                {
                    var controllerInstance = CreateController<TController>(request);
                    return (HttpResponse)action.Invoke(controllerInstance, null);
                });
            }

            return routingTable;
        }
          
        public static IRoutingTable MapPost<TController>(
            this IRoutingTable routingTable,
            string path,
            Func<TController, HttpResponse> controllerFunction)
            where TController : Controller
            => routingTable.MapPost(path, request => controllerFunction(
                CreateController<TController>(request)));

        //public static IRoutingTable MapControllers(this IRoutingTable routingTable)
        //{
        //    var controllerActions = GetControllerActions();
        //
        //    foreach (var controllerAction in controllerActions)
        //    {
        //        var controllerName = controllerAction.DeclaringType.GetControllerName();
        //        var actionName = controllerAction.Name;
        //
        //        var path = $"/{controllerName}/{actionName}";
        //
        //        var responseFunction = GetResponseFunction(controllerAction);
        //
        //        var httpMethod = HttpMethod.Get;
        //
        //        var httpMethodAttribute = controllerAction
        //            .GetCustomAttribute<HttpMethodAttribute>();
        //
        //        if (httpMethodAttribute != null)
        //        {
        //            httpMethod = httpMethodAttribute.HttpMethod;
        //        }
        //
        //        routingTable.Map(httpMethod, path, responseFunction);
        //
        //        MapDefaultRoutes(
        //            routingTable, 
        //            httpMethod, 
        //            controllerName, 
        //            actionName, 
        //            responseFunction);
        //    }
        //
        //    return routingTable;
        //}

        private static IEnumerable<MethodInfo> GetControllerActions()
            => Assembly
                .GetEntryAssembly()
                .GetExportedTypes()
                .Where(t => !t.IsAbstract 
                    && t.IsAssignableTo(typeof(Controller))
                    && t.Name.EndsWith(nameof(Controller)))
                .SelectMany(t => t
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m.ReturnType.IsAssignableTo(httpResponseType)))
                .ToList();

        private static Func<HttpRequest, HttpResponse> GetResponseFunction(MethodInfo controllerAction)
            => request =>
            {
                if (!UserIsAuthorized(controllerAction, request.Session))
                {
                    return new HttpResponse(HttpStatusCode.Unauthorized);
                }
                        
                var controllerInstance = CreateController(controllerAction.DeclaringType, request);

                var parameterValues = GetParameterValues(controllerAction, request);

                return (HttpResponse)controllerAction.Invoke(controllerInstance, parameterValues);
            };

        private static TController CreateController<TController>(HttpRequest request)
            where TController : Controller
            => (TController)CreateController(typeof(TController), request);

        private static Controller CreateController(Type controllerType, HttpRequest request)
        {
            var controller = (Controller)Activator.CreateInstance(controllerType);

            controllerType
                .GetProperty("Request", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(controller, request);

            return controller;
        }

        private static void MapDefaultRoutes(
            IRoutingTable routingTable,
            HttpMethod httpMethod,
            string controllerName,
            string actionName,
            Func<HttpRequest, HttpResponse> responseFunction)
        {
            const string defaultActionName = "Index";
            const string defaultControllerName = "Home";

            if (actionName == defaultActionName)
            {
                routingTable.Map(httpMethod, $"/{controllerName}", responseFunction);

                if (controllerName == defaultControllerName)
                {
                    routingTable.Map(httpMethod, "/", responseFunction);
                }
            }
        }

        private static bool UserIsAuthorized(
            MethodInfo controllerAction,
            HttpSession session)
        {
            var authorizationRequired = controllerAction
                .DeclaringType
                .GetCustomAttribute<AuthorizeAttribute>()
                ?? controllerAction
                .GetCustomAttribute<AuthorizeAttribute>();

            if (authorizationRequired != null)
            {
                var userIsAuthorized = session.ContainsKey(Controller.UserSessionKey)
                    && session[Controller.UserSessionKey] != null;

                if (!userIsAuthorized)
                {
                    return false;
                }
            }

            return true;
        }

        private static object[] GetParameterValues(
            MethodInfo controllerAction,
            HttpRequest request)
        {
            var actionParameters = controllerAction
                .GetParameters()
                .Select(p => new
                {
                    p.Name,
                    Type = p.ParameterType
                })
                .ToArray();

            var parameterValues = new object[actionParameters.Length];

            for (int i = 0; i < actionParameters.Length; i++)
            {
                var parameter = actionParameters[i];
                var parameterName = parameter.Name;
                var parameterType = parameter.Type;

                if (parameterType.IsPrimitive || parameterType == stringType)
                {
                    var parameterValue = request.GetValue(parameterName);

                    parameterValues[i] = Convert.ChangeType(parameterValue, parameterType);
                }
                else
                {
                    var parameterValue = Activator.CreateInstance(parameterType);

                    var parameterProperties = parameterType.GetProperties();

                    foreach (var property in parameterProperties)
                    {
                        var propertyValue = request.GetValue(property.Name);

                        property.SetValue(
                            parameterValue, 
                            Convert.ChangeType(propertyValue, property.PropertyType));
                    }

                    parameterValues[i] = parameterValue;
                }
            }

            return parameterValues;
        }

        private static string GetValue(this HttpRequest request, string name)
            => request.Query.GetValueOrDefault(name) ??
                request.Form.GetValueOrDefault(name);
    }
}
