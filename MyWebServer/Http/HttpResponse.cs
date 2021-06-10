namespace MyWebServer.Http
{
    using MyWebServer.Common;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class HttpResponse
    {
        public HttpResponse(HttpStatusCode statusCode)
        {
            this.StatusCode = statusCode;

            this.AddHeader(HttpHeader.Server, "My Web Server");
            this.AddHeader(HttpHeader.Date, $"{DateTime.UtcNow:r}");
        }

        public HttpStatusCode StatusCode { get; protected set; }

        public IDictionary<string, HttpHeader> Headers { get; } = new Dictionary<string, HttpHeader>();

        public IDictionary<string, HttpCookie> Cookies { get; } = new Dictionary<string, HttpCookie>();

        public string Content { get; protected set; }

        public void AddHeader(string name, string value)
        {
            Guard.AgainstNull(name, nameof(name));
            Guard.AgainstNull(value, nameof(value));

            this.Headers[name] = new HttpHeader(name, value);
        }

        public void AddCookie(string name, string value)
        {
            Guard.AgainstNull(name, nameof(name));
            Guard.AgainstNull(value, nameof(value));

            this.Cookies[name] = new HttpCookie(name, value);
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            result.AppendLine($"HTTP/1.1 {(int)this.StatusCode} {this.StatusCode}");

            foreach (var header in this.Headers.Values)
            {
                result.AppendLine(header.ToString());
            }

            foreach (var cookie in this.Cookies.Values)
            {
                result.AppendLine($"{HttpHeader.SetCookie}: {cookie}");
            }

            if (!string.IsNullOrEmpty(this.Content))
            {
                result.AppendLine();

                result.Append(this.Content);
            }

            return result.ToString();
        }

        protected void PrepareContent(string content, string contentType)
        {
            Guard.AgainstNull(content, nameof(content));
            Guard.AgainstNull(content, nameof(contentType));

            var contentLength = Encoding.UTF8.GetByteCount(content).ToString();

            this.AddHeader(HttpHeader.ContentType, contentType);
            this.AddHeader(HttpHeader.ContentLength, contentLength);

            this.Content = content;
        }
    }
}
