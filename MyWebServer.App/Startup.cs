namespace MyWebServer.App
{
    using MyWebServer;
    using MyWebServer.App.Controllers;
    using MyWebServer.Controllers;
    using System.Threading.Tasks;

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
