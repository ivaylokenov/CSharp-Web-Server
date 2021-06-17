namespace MyWebServer.Demo.Controllers
{
    using MyWebServer.Demo.Models.Animals;
    using MyWebServer.Controllers;
    using MyWebServer.Http;

    public class DogsController : Controller
    {
        [HttpGet]
        public HttpResponse Create() => View();

        [HttpPost]
        public HttpResponse Create(DogFormModel model)
            => Text($"Dog: {model.Name} - {model.Age} - {model.Breed}");
    }
}
