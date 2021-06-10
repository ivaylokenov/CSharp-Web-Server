namespace MyWebServer.Responses
{
    using MyWebServer.Http;

    public class HtmlResponse : ContentResponse
    {
        public HtmlResponse(string html) 
            : base(html, HttpContentType.Html)
        {
        }
    }
}
