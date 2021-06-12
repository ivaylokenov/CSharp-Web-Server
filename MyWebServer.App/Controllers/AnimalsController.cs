namespace MyWebServer.App.Controllers
{
    using MyWebServer.App.Models.Animals;
    using MyWebServer.Controllers;
    using MyWebServer.Http;

    public class AnimalsController : Controller
    {
        public AnimalsController(HttpRequest request) 
            : base(request)
        {
        }

        public HttpResponse Cats()
        {
            const string nameKey = "Name"; 
            const string ageKey = "age";

            var query = this.Request.Query;

            var catName = query.Contains(nameKey) // Made this case insensitive 
                ? query[nameKey] // This as well (if this should be removed both the ToLower()-s in QueryCollection class at the indexer and Contains() should be removed)
                : "the cats";

            var catAge = query.Contains(ageKey)
                ? int.Parse(query[ageKey])
                : 0;

            var viewModel = new CatViewModel
            {
                Name = catName,
                Age = catAge
            };

            return View(viewModel);
        }

        public HttpResponse Dogs() => View();

        public HttpResponse Bunnies() => View("Rabbits");

        public HttpResponse Turtles() => View("Animals/Wild/Turtles");
    }
}
