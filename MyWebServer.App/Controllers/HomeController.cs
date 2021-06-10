namespace MyWebServer.App.Controllers
{
    using MyWebServer.Controllers;
    using MyWebServer.Http;

    public class HomeController : Controller
    {
        public HomeController(HttpRequest request) 
            : base(request)
        {
        }

        public HttpResponse Index() => Text("Hello from Ivo!");

        public HttpResponse LocalRedirect() => Redirect("/Animals/Cats");

        public HttpResponse ToSoftUni() => Redirect("https://softuni.bg");
    }
}
