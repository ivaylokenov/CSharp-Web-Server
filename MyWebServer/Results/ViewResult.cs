namespace MyWebServer.Results
{
    using System.IO;
    using MyWebServer.Http;
    using MyWebServer.Results.Views;

    public class ViewResult : ActionResult
    {
        private const char PathSeparator = '/';
        private readonly string[] ViewFileExtensions = { "html", "cshtml" };

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

            var (viewPath, viewExists) = FindView(viewName);

            if (!viewExists)
            {
                this.PrepareMissingViewError(viewPath);

                return;
            }

            var viewContent = File.ReadAllText(viewPath);

            var (layoutPath, layoutExists) = FindLayout();

            if (layoutExists)
            {
                var layoutContent = File.ReadAllText(layoutPath);

                viewContent = layoutContent.Replace("@RenderBody()", viewContent);
            }

            viewContent = viewEngine.RenderHtml(viewContent, model, userId);

            this.SetContent(viewContent, HttpContentType.Html);
        }

        private (string, bool) FindView(string viewName)
        {
            string viewPath = null;
            var exists = false;

            foreach (var fileExtension in ViewFileExtensions)
            {
                viewPath = Path.GetFullPath($"./Views/" + viewName.TrimStart(PathSeparator) + $".{fileExtension}");

                if (File.Exists(viewPath))
                {
                    exists = true;
                    break;
                }
            }

            return (viewPath, exists);
        }

        private (string, bool) FindLayout()
        {
            string layoutPath = null;
            bool exists = false;

            foreach (var fileExtension in ViewFileExtensions)
            {
                layoutPath = Path.GetFullPath($"./Views/Layout.{fileExtension}");

                if (File.Exists(layoutPath))
                {
                    exists = true;
                    break;
                }

                layoutPath = Path.GetFullPath($"./Views/Shared/_Layout.{fileExtension}");

                if (File.Exists(layoutPath))
                {
                    exists = true;
                    break;
                }
            }

            return (layoutPath, exists);
        }

        private void PrepareMissingViewError(string viewPath)
        {
            this.StatusCode = HttpStatusCode.NotFound;

            var errorMessage = $"View '{viewPath}' was not found.";

            this.SetContent(errorMessage, HttpContentType.PlainText);
        }
    }
}
