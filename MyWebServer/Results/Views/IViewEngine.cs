namespace MyWebServer.Results.Views
{
    public interface IViewEngine
    {
        string RenderHtml(string content, object model, string userId);
    }
}
