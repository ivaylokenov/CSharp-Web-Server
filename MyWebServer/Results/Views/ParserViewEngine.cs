namespace MyWebServer.Results.Views
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Text;

    public class ParserViewEngine : IViewEngine
    {
        public string RenderHtml(string content, object model, string userId)
        {
            if (model is not IEnumerable)
            {
                content = PopulateModelProperties(content, "Model", model);
            }

            var result = new StringBuilder();

            var lines = content
                .Split(Environment.NewLine)
                .Select(line => line.Trim());

            var inLoop = false;
            string loopModelName = null;
            StringBuilder loopContent = null;

            foreach (var line in lines)
            {
                if (line.StartsWith("@foreach"))
                {
                    if (model is not IEnumerable)
                    {
                        throw new InvalidOperationException("Using a foreach in the loop requires the view model to be a collection.");
                    }

                    inLoop = true;

                    loopModelName = line
                        .Split()
                        .SkipWhile(l => l.Contains("var"))
                        .Skip(2)
                        .FirstOrDefault();

                    if (loopModelName == null)
                    {
                        throw new InvalidOperationException("The foreach statement in the view is not valid.");
                    }

                    continue;
                }

                if (inLoop)
                {
                    if (line.StartsWith("{"))
                    {
                        loopContent = new StringBuilder();
                    }
                    else if (line.StartsWith("}"))
                    {
                        var loopTemplate = loopContent.ToString();

                        foreach (var item in (IEnumerable)model)
                        {
                            var loopResult = PopulateModelProperties(loopTemplate, loopModelName, item);

                            result.AppendLine(loopResult);
                        }

                        inLoop = false;
                    }
                    else
                    {
                        loopContent.AppendLine(line);
                    }

                    continue;
                }

                result.AppendLine(line);
            }

            return result.ToString();
        }

        private static string PopulateModelProperties(string content, string modelName, object model)
        {
            if (model == null)
            {
                return content;
            }

            var data = model
                .GetType()
                .GetProperties()
                .Select(pr => new
                {
                    pr.Name,
                    Value = pr.GetValue(model)
                });

            foreach (var entry in data)
            {
                content = content.Replace($"@{modelName}.{entry.Name}", entry.Value.ToString());
            }

            return content;
        }
    }
}
