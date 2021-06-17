namespace MyWebServer.Results
{
    using MyWebServer.Http;

    public class UnauthorizedResult : ActionResult
    {
        public UnauthorizedResult(HttpResponse response)
            : base(response)
            => this.StatusCode = HttpStatusCode.Unauthorized;
    }
}
