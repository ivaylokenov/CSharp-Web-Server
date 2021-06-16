namespace MyWebServer.Results.Views
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

    public class CompilationViewEngine : IViewEngine
    {
        public string RenderHtml(string content, object model, string userId)
        {
            var csharpCode = GenerateCSharpFromTemplate(content, model);
            var executableObject = GenerateExecutableCоde(csharpCode, model);
            var html = executableObject.ExecuteTemplate(model, userId);

            return html;
        }

        private string GenerateCSharpFromTemplate(string templateCode, object viewModel)
        {
            var typeOfModel = "object";

            if (viewModel != null)
            {
                if (viewModel.GetType().IsGenericType)
                {
                    var modelName = viewModel.GetType().FullName;
                    var genericArguments = viewModel.GetType().GenericTypeArguments.Select(x => x.FullName);

                    typeOfModel = modelName.Substring(0, modelName.IndexOf('`')) + "<" + string.Join(",", genericArguments) + ">";
                }
                else
                {
                    typeOfModel = viewModel.GetType().FullName;
                }
            }

            var csharpCode = @"
namespace ViewNamespace
{
    using System;
    using System.Text;
    using System.Linq;
    using System.Collections.Generic;
    using MyWebServer.Results.Views;

    public class ViewClass : IView
    {
        public string ExecuteTemplate(object viewModel, string user)
        {
            var User = user;
            var Model = viewModel as " + typeOfModel + @";
            var html = new StringBuilder();

            " + GetMethodBody(templateCode) + @"

            return html.ToString();
        }
    }
}
";
            return csharpCode;
        }

        private string GetMethodBody(string templateCode)
        {
            var csharpCodeRegex = new Regex(@"[^\""\s&\'\<]+");
            var supportedOperators = new List<string> { "for", "while", "if", "else", "foreach" };
            var csharpCode = new StringBuilder();
            var sr = new StringReader(templateCode);
            string line = null;

            while ((line = sr.ReadLine()) != null)
            {
                if (supportedOperators.Any(x => line.TrimStart().StartsWith("@" + x)))
                {
                    var atSignLocation = line.IndexOf("@");
                    line = line.Remove(atSignLocation, 1);
                    csharpCode.AppendLine(line);
                }
                else if (line.TrimStart().StartsWith("{") ||
                    line.TrimStart().StartsWith("}"))
                {
                    csharpCode.AppendLine(line);
                }
                else
                {
                    csharpCode.Append($"html.AppendLine(@\"");

                    while (line.Contains("@"))
                    {
                        var atSignLocation = line.IndexOf("@");
                        var htmlBeforeAtSign = line.Substring(0, atSignLocation);
                        csharpCode.Append(htmlBeforeAtSign.Replace("\"", "\"\"") + "\" + ");
                        var lineAfterAtSign = line.Substring(atSignLocation + 1);
                        var code = csharpCodeRegex.Match(lineAfterAtSign).Value;
                        csharpCode.Append(code + " + @\"");
                        line = lineAfterAtSign.Substring(code.Length);
                    }

                    csharpCode.AppendLine(line.Replace("\"", "\"\"") + "\");");
                }
            }

            return csharpCode.ToString();
        }

        private IView GenerateExecutableCоde(string csharpCode, object viewModel)
        {
            var compileResult = CSharpCompilation.Create("ViewAssembly")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(IView).Assembly.Location));

            if (viewModel != null)
            {
                if (viewModel.GetType().IsGenericType)
                {
                    var genericArguments = viewModel.GetType().GenericTypeArguments;

                    foreach (var genericArgument in genericArguments)
                    {
                        compileResult = compileResult
                            .AddReferences(MetadataReference.CreateFromFile(genericArgument.Assembly.Location));
                    }
                }

                compileResult = compileResult
                    .AddReferences(MetadataReference.CreateFromFile(viewModel.GetType().Assembly.Location));
            }

            var libraries = Assembly.Load(
                new AssemblyName("netstandard")).GetReferencedAssemblies();

            foreach (var library in libraries)
            {
                compileResult = compileResult
                    .AddReferences(MetadataReference.CreateFromFile(
                        Assembly.Load(library).Location));
            }

            compileResult = compileResult.AddSyntaxTrees(SyntaxFactory.ParseSyntaxTree(csharpCode));

            using var memoryStream = new MemoryStream();
            var result = compileResult.Emit(memoryStream);

            if (!result.Success)
            {
                return new ErrorView(result.Diagnostics
                    .Where(x => x.Severity == DiagnosticSeverity.Error)
                    .Select(x => x.GetMessage()), csharpCode);
            }

            try
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                var byteAssembly = memoryStream.ToArray();

                var assembly = Assembly.Load(byteAssembly);
                var viewType = assembly.GetType("ViewNamespace.ViewClass");

                var instance = Activator.CreateInstance(viewType);

                return (instance as IView) ?? new ErrorView(new List<string> { "Instance is null." }, csharpCode);
            }
            catch (Exception ex)
            {
                return new ErrorView(new List<string> { ex.ToString() }, csharpCode);
            }
        }
    }
}
