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
                //No support for post functionality to the MapControllers() method as of the moment.
                //More info in the MapControllers method itself.
                .MapPost<CatsController>("/Cats/Save", c => c.Save()))
                //.MapControllers()
                .MapGet<HomeController>("/ToCats", c => c.LocalRedirect()))
            .Start();
    }
}
