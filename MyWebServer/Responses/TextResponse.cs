namespace MyWebServer.Responses
{
    using MyWebServer.Http;

    public class TextResponse : ContentResponse
    {
        public TextResponse(string text)
            : base(text, HttpContentType.PlainText)
        {
        }
    }
}
