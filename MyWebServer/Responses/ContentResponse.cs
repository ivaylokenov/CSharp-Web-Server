namespace MyWebServer.Responses
{
    using MyWebServer.Http;

    public class ContentResponse : HttpResponse
    {
        public ContentResponse(string content, string contentType)
            : base(HttpStatusCode.OK) 
            => this.PrepareContent(content, contentType);
    }
}
