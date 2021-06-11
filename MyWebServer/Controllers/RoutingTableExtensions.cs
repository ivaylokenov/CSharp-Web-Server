namespace MyWebServer.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using MyWebServer.Http;
    using MyWebServer.Routing;

    public static class RoutingTableExtensions
    {
        private static Type httpResponseType = typeof(HttpResponse);

        public static IRoutingTable MapGet<TController>(
            this IRoutingTable routingTable,
            string path,
            Func<TController, HttpResponse> controllerFunction)
            where TController : Controller
            => routingTable.MapGet(path, request => controllerFunction(
                CreateController<TController>(request)));

        public static IRoutingTable MapPost<TController>(
            this IRoutingTable routingTable,
            string path,
            Func<TController, HttpResponse> controllerFunction)
            where TController : Controller
            => routingTable.MapPost(path, request => controllerFunction(
                CreateController<TController>(request)));

        public static IRoutingTable MapControllers(this IRoutingTable routingTable)
        {
            var controllerActions = GetControllerActions();

            foreach (var controllerAction in controllerActions)
            {
                var controllerName = controllerAction.DeclaringType.GetControllerName();
                var actionName = controllerAction.Name;

                var path = $"/{controllerName}/{actionName}";

                var responseFunction = GetResponseFunction(controllerAction);

                var httpMethod = HttpMethod.Get;

                var httpMethodAttribute = controllerAction
                    .GetCustomAttribute<HttpMethodAttribute>();

                if (httpMethodAttribute != null)
                {
                    httpMethod = httpMethodAttribute.HttpMethod;
                }

                routingTable.Map(httpMethod, path, responseFunction);

                MapDefaultRoutes(
                    routingTable, 
                    httpMethod, 
                    controllerName, 
                    actionName, 
                    responseFunction);
            }

            return routingTable;
        }

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

                return (HttpResponse)controllerAction.Invoke(controllerInstance, Array.Empty<object>());
            };

        private static Controller CreateController(Type controller, HttpRequest request)
            => (Controller)Activator.CreateInstance(controller, new[] { request });

        private static TController CreateController<TController>(HttpRequest request)
            where TController : Controller
            => (TController)CreateController(typeof(TController), request);
    
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
    }
}
