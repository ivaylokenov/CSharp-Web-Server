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

                routingTable.MapGet(path, responseFunction);

                MapDefaultRoutes(routingTable, controllerName, actionName, responseFunction);
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
            string controllerName,
            string actionName,
            Func<HttpRequest, HttpResponse> responseFunction)
        {
            const string defaultActionName = "Index";
            const string defaultControllerName = "Home";

            if (actionName == defaultActionName)
            {
                routingTable.MapGet($"/{controllerName}", responseFunction);

                if (controllerName == defaultControllerName)
                {
                    routingTable.MapGet("/", responseFunction);
                }
            }
        }
    }
}
