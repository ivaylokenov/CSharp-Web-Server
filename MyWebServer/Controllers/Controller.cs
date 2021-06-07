namespace CSharpWebServer.Server.Controllers
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using CSharpWebServer.Server.Http;
    using CSharpWebServer.Server.Responses;

    public abstract class Controller
    {
        protected HttpRequest Request { get; private init; }

        protected Controller(HttpRequest request)
        => this.Request = request;

        protected HttpResponse Text(string text)
            => new TextResponse(text);

        protected HttpResponse Html(string text)
            => new HtmlResponse(text);

        protected HttpResponse Redirect(string location)
            => new RedirectResponse(location);

        protected HttpResponse View()
        {
            StackTrace trace = new StackTrace();
            StackFrame frame = trace.GetFrame(1); // 0 will be the inner-most method
            MethodBase method = frame.GetMethod();
            var controllerName = GetNameOfController(method.DeclaringType.Name).Trim();
            if (controllerName == null) new NotFoundResponse();

            var methodName = trace.GetFrame(1).GetMethod().Name.Trim();

            string codeBase = Assembly.GetExecutingAssembly().Location;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            var dirName =  Path.GetDirectoryName(path);

            var pathToFile = Path.GetFullPath(Path.Combine(dirName,@"..\..\..\","Views",controllerName));

            if (!Directory.Exists(pathToFile)) return new NotFoundResponse();

            var filePath = Directory.GetFiles(pathToFile, $"{methodName.ToLower()}.cshtml")[0];
            var html = File.ReadAllText(filePath);
            return new HtmlResponse(html);
        }
        private string GetNameOfController(string fullname)
        {
            if (fullname.Length <= 10)
            {
                return null;
            }
            var indexOfControllerWord = fullname.IndexOf("Controller");
            return fullname.Substring(0, indexOfControllerWord);
        }
    }
}
