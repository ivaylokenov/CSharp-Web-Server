namespace MyWebServer.Results
{
    using MyWebServer.Http;

    public class BadRequestResult : HttpResponse
    {
        public BadRequestResult() 
            : base(HttpStatusCode.BadRequest)
        {
        }
    }
}
