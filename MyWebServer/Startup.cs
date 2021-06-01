namespace MyWebServer
{
    using System.Threading.Tasks;
    using MyWebServer.Server;
    using MyWebServer.Server.Responses;

    public class Startup
    {
        public static async Task Main()
            => await new HttpServer(routes => routes
                .MapGet("/", new TextResponse("Hello from Ivo!"))
                .MapGet("/Cats", new TextResponse("<h1>Hello from the cats!</h1>", "text/html"))
                .MapGet("/Dogs", new TextResponse("<h1>Hello from the dogs!</h1>", "text/html")))
            .Start();
    }
}
