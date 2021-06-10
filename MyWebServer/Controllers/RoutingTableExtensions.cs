namespace MyWebServer.Controllers
{
    using MyWebServer.Http;
    using MyWebServer.Routing;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public static class RoutingTableExtensions
    {
        //TODO: Extract constants in a separate class. Backslash repeats in other files.
        private const string ControllerKeyword = "Controller";
        private const string Backslash = "/";
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
                .Where(t => t.BaseType == typeof(Controller) && t.Name.EndsWith(ControllerKeyword));

            foreach (var controller in controllerTypes)
            {
                var genericGet = typeof(RoutingTableExtensions).GetMethod("MapGet", new Type[] { routingTable.GetType() }).MakeGenericMethod(controller);
                //Static method, no object instance => null.
                genericGet.Invoke(null, new object[] { routingTable });

                //TODO: Set post routes as well.
                //Currently can't find any specific characteristic to them that I can use reflection to find.
            }

            return routingTable;
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

        private static TController CreateController<TController>(HttpRequest request)
            => (TController)Activator.CreateInstance(typeof(TController), new[] { request });
    }
}
