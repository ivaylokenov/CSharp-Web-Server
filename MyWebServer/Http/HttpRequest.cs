namespace MyWebServer.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using MyWebServer.Http.Collections;

    public class HttpRequest
    {
        private static Dictionary<string, HttpSession> Sessions = new();

        private const string NewLine = "\r\n";

        public HttpMethod Method { get; private set; }

        public string Path { get; private set; }

        public QueryCollection Query { get; private set; }

        public HeaderCollection Headers { get; private set; }

        public IReadOnlyDictionary<string, HttpCookie> Cookies { get; private set; }

        public FormCollection Form { get; private set; }

        public string Body { get; private set; }

        public HttpSession Session { get; private set; }

        public static HttpRequest Parse(string request)
        {
            var lines = request.Split(NewLine);

            var startLine = lines.First().Split(" ");

            var method = ParseMethod(startLine[0]);
            var url = startLine[1];

            var (path, query) = ParseUrl(url);

            var headers = ParseHeaders(lines.Skip(1));

            var cookies = ParseCookies(headers);

            var session = GetSession(cookies);

            var bodyLines = lines.Skip(headers.Count + 2).ToArray();

            var body = string.Join(NewLine, bodyLines);

            var form = ParseForm(headers, body);

            return new HttpRequest
            {
                Method = method,
                Path = path,
                Query = query,
                Headers = headers,
                Cookies = cookies,
                Session = session,
                Body = body,
                Form = form
            };
        }

        private static HttpMethod ParseMethod(string method)
        {
            try
            {
                return (HttpMethod)Enum.Parse(typeof(HttpMethod), method, true);
            }
            catch (Exception)
            {
                throw new InvalidOperationException($"Method '{method}' is not supported");
            }
        }

        private static (string, QueryCollection) ParseUrl(string url)
        {
            var urlParts = url.Split('?', 2);

            var path = urlParts[0];
            var query = urlParts.Length > 1
                ? new QueryCollection(ParseQuery(urlParts[1]))
                : new QueryCollection();

            return (path, query);
        }

        private static Dictionary<string, string> ParseQuery(string queryString) 
            => HttpUtility.UrlDecode(queryString)
                .Split('&')
                .Select(part => part.Split('='))
                .Where(part => part.Length == 2)
                .ToDictionary(
                    part => part[0], 
                    part => part[1], 
                    StringComparer.InvariantCultureIgnoreCase);

        private static HeaderCollection ParseHeaders(IEnumerable<string> headerLines)
        {
            var headerCollection = new Dictionary<string, HttpHeader>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var headerLine in headerLines)
            {
                if (headerLine == string.Empty)
                {
                    break;
                }

                var headerParts = headerLine.Split(":", 2);

                if (headerParts.Length != 2)
                {
                    throw new InvalidOperationException("Request is not valid.");
                }

                var headerName = headerParts[0];
                var headerValue = headerParts[1].Trim();

                var header = new HttpHeader(headerName, headerValue);

                headerCollection[headerName] = header;
            }

            return new HeaderCollection(headerCollection);
        }

        private static Dictionary<string, HttpCookie> ParseCookies(HeaderCollection headers)
        {
            var cookieCollection = new Dictionary<string, HttpCookie>(StringComparer.InvariantCultureIgnoreCase);

            if (headers.Contains(HttpHeader.Cookie))
            {
                var cookieHeader = headers[HttpHeader.Cookie];

                var allCookies = cookieHeader.Split(';');

                foreach (var cookieText in allCookies)
                {
                    var cookieParts = cookieText.Split('=');

                    var cookieName = cookieParts[0].Trim();
                    var cookieValue = cookieParts[1].Trim();

                    var cookie = new HttpCookie(cookieName, cookieValue);

                    cookieCollection[cookieName] = cookie;
                }
            }

            return cookieCollection;
        }

        private static HttpSession GetSession(Dictionary<string, HttpCookie> cookies)
        {
            var sessionId = cookies.ContainsKey(HttpSession.SessionCookieName)
                ? cookies[HttpSession.SessionCookieName].Value
                : Guid.NewGuid().ToString();

            if (!Sessions.ContainsKey(sessionId))
            {
                Sessions[sessionId] = new HttpSession(sessionId) 
                { 
                    IsNew = true 
                };
            }

            return Sessions[sessionId];
        }

        private static FormCollection ParseForm(HeaderCollection headers, string body)
        {
            var result = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            if (headers.Contains(HttpHeader.ContentType)
                && headers[HttpHeader.ContentType] == HttpContentType.FormUrlEncoded)
            {
                result = ParseQuery(body);
            }

            return new FormCollection(result);
        }
    }
}
