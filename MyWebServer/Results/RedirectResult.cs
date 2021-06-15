namespace MyWebServer.Results
{
    using MyWebServer.Http;

    public class RedirectResult : ActionResult
    {
        public RedirectResult(HttpResponse response, string location)
            : base(response)
        {
            this.StatusCode = HttpStatusCode.Found;

            this.Headers.Add(HttpHeader.Location, location);
        }
    }
}
