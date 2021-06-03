namespace MyWebServer.Responses
{
    using MyWebServer.Http;

    public class NotFoundResponse : HttpResponse
    {
        public NotFoundResponse()
            : base(HttpStatusCode.NotFound)
        {
        }
    }
}
