namespace MyWebServer.Demo.Controllers
{
    using System;
    using MyWebServer.Demo.Models.Animals;
    using MyWebServer.Controllers;
    using MyWebServer.Http;

    public class HomeController : Controller
    {
        public HttpResponse Index() => Text("Hello from Ivo!");

        public HttpResponse LocalRedirect() => Redirect("/Animals/Cats");

        public HttpResponse ToSoftUni() => Redirect("https://softuni.bg");

        public HttpResponse StaticFiles() => View();

        public HttpResponse HtmlView() => View(new CatViewModel { Name = "Sharo", Age = 5 });

        public HttpResponse MissingView() => View();

        public HttpResponse Error() => throw new InvalidOperationException("Invalid action!");
    }
}
