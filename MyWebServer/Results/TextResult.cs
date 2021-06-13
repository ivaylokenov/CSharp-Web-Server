namespace MyWebServer.Results
{
    using MyWebServer.Http;

    public class TextResult : ContentResult
    {
        public TextResult(HttpResponse response, string text)
            : base(response, text, HttpContentType.PlainText)
        {
        }
    }
}
