namespace MyWebServer.App
{
    using System.Threading.Tasks;
    using MyWebServer;
    using MyWebServer.Controllers;
    using MyWebServer.App.Controllers;

    public class Startup
    {
        public static async Task Main()
            => await new HttpServer(routes => routes
                .MapStaticFiles()
                .MapControllers()
                .MapGet<HomeController>("/ToCats", c => c.LocalRedirect()))
            .Start();
    }
}
