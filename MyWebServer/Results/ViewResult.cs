namespace MyWebServer.Results
{
    using System.IO;
    using MyWebServer.Http;
    using MyWebServer.Results.Views;

    public class ViewResult : ActionResult
    {
        private const char PathSeparator = '/';

        public ViewResult(
            HttpResponse response,
            IViewEngine viewEngine,
            string viewName, 
            string controllerName, 
            object model,
            string userId)
            : base(response) 
            => this.GetHtml(viewEngine, viewName, controllerName, model, userId);

        private void GetHtml(IViewEngine viewEngine, string viewName, string controllerName, object model, string userId)
        {
            if (!viewName.Contains(PathSeparator))
            {
                viewName = controllerName + PathSeparator + viewName;
            }

            var viewPath = Path.GetFullPath($"./Views/" + viewName.TrimStart(PathSeparator) + ".cshtml");

            if (!File.Exists(viewPath))
            {
                this.PrepareMissingViewError(viewPath);

                return;
            }

            var viewContent = File.ReadAllText(viewPath);

            var layoutPath = Path.GetFullPath("./Views/Layout.cshtml");

            if (File.Exists(layoutPath))
            {
                var layoutContent = File.ReadAllText(layoutPath);

                viewContent = layoutContent.Replace("@RenderBody()", viewContent);
            }

            if (model != null)
            {
                viewContent = viewEngine.RenderHtml(viewContent, model, userId);
            }

            this.SetContent(viewContent, HttpContentType.Html);
        }

        private void PrepareMissingViewError(string viewPath)
        {
            this.StatusCode = HttpStatusCode.NotFound;

            var errorMessage = $"View '{viewPath}' was not found.";

            this.SetContent(errorMessage, HttpContentType.PlainText);
        }
    }
}
