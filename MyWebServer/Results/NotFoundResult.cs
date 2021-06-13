namespace MyWebServer.Results
{
    using MyWebServer.Http;

    public class NotFoundResult : ActionResult
    {
        public NotFoundResult(HttpResponse response)
            : base(response) 
            => this.StatusCode = HttpStatusCode.NotFound;
    }
}
