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
        public static IRoutingTable MapGet<TController>(
            this IRoutingTable routingTable,
            string path,
            Func<TController, HttpResponse> controllerFunction)
            where TController : Controller
            => routingTable.MapGet(path, request => controllerFunction(
                CreateController<TController>(request)));

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
                string path = (action.Name == "Index" || action.Name == string.Empty) ? "/" : "/" + action.Name;

                routingTable.MapGet(path, request =>
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

        private static TController CreateController<TController>(HttpRequest request)
            => (TController)Activator.CreateInstance(typeof(TController), new[] { request });
    }
}
