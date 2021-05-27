namespace MyWebServer.Server.Http
{
    public class HttpResponse
    {
        public HttpStatusCode StatusCode { get; init; }

        public HttpHeaderCollection Headers { get; } = new HttpHeaderCollection();
    
        public string Content { get; init; }
    }
}
