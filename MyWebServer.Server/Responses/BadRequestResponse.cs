namespace MyWebServer.Server.Responses
{
    using MyWebServer.Server.Http;

    public class BadRequestResponse : HttpResponse
    {
        public BadRequestResponse() 
            : base(HttpStatusCode.BadRequest)
        {
        }
    }
}
