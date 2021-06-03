namespace MyWebServer.Responses
{
    public class HtmlResponse : ContentResponse
    {
        public HtmlResponse(string html) 
            : base(html, "text/html; charset=UTF-8")
        {
        }
    }
}
