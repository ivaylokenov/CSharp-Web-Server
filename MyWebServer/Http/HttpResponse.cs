namespace MyWebServer.Http
{
    using System;
    using System.Text;

    public abstract class HttpResponse
    {
        protected HttpResponse(HttpStatusCode statusCode)
        {
            this.StatusCode = statusCode;

            this.Headers.Add("Server", "My Web Server");
            this.Headers.Add("Date", $"{DateTime.UtcNow:r}");
        }

        public HttpStatusCode StatusCode { get; init; }

        public HttpHeaderCollection Headers { get; } = new HttpHeaderCollection();

        public string Content { get; init; }

        public override string ToString()
        {
            var result = new StringBuilder();

            result.Append($"HTTP/1.1 {(int)this.StatusCode} {this.StatusCode}"
                          + HttpConstants.NewLine);

            foreach (var header in this.Headers)
            {
                result.Append(header + HttpConstants.NewLine);
            }


            if (!string.IsNullOrEmpty(this.Content))
            {
                result.Append(HttpConstants.NewLine);

                result.Append(this.Content);
            }

            return result.ToString();
        }
    }
}
