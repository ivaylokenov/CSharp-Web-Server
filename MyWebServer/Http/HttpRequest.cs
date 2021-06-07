namespace MyWebServer.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class HttpRequest
    {
        public static Dictionary<string, Dictionary<string, string>>
            Sessions = new();

        public HttpRequest() 
        {
            this.Cookies = new List<Cookie>(); // I am not sure if this is needed
        }

        public HttpMethod Method { get; private set; }

        public string Path { get; private set; }

        public Dictionary<string, string> Query { get; private set; }

        public Dictionary<string, string> Session { get; set; }

        public ICollection<Cookie> Cookies { get; private set; }

        public HttpHeaderCollection Headers { get; private set; }

        public string Body { get; private set; }

        public static HttpRequest Parse(string request)
        {
            var lines = request.Split(HttpConstants.NewLine);

            var startLine = lines.First().Split(" ");

            var method = ParseHttpMethod(startLine[0]);
            var url = startLine[1];

            var (path, query) = ParseUrl(url);

            var headers = ParseHttpHeaders(lines.Skip(1));

            var bodyLines = lines.Skip(headers.Count + 2).ToArray();

            var body = string.Join(HttpConstants.NewLine, bodyLines);

            var cookiesAndSession = GetCookiesAndSession(headers);

            return new HttpRequest
            {
                Method = method,
                Path = path,
                Query = query,
                Headers = headers,
                Body = body,
                Cookies = cookiesAndSession.Item1,
                Session = cookiesAndSession.Item2
            };
        }

        private static HttpMethod ParseHttpMethod(string method) 
            => method.ToUpper() switch
            {
                "GET" => HttpMethod.Get,
                "POST" => HttpMethod.Post,
                "PUT" => HttpMethod.Put,
                "DELETE" => HttpMethod.Delete,
                _ => throw new InvalidOperationException($"Method '{method}' is not supported."),
            };

        private static (string, Dictionary<string, string>) ParseUrl(string url)
        {
            var urlParts = url.Split('?', 2);

            var path = urlParts[0];
            var query = urlParts.Length > 1
                ? ParseQuery(urlParts[1])
                : new Dictionary<string, string>();

            return (path, query);
        }

        private static Dictionary<string, string> ParseQuery(string queryString) 
            => queryString
                .Split('&')
                .Select(part => part.Split('='))
                .Where(part => part.Length == 2)
                .ToDictionary(part => part[0], part => part[1]);

        private static HttpHeaderCollection ParseHttpHeaders(IEnumerable<string> headerLines)
        {
            var headerCollection = new HttpHeaderCollection();

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

                headerCollection.Add(headerName, headerValue);
            }

            return headerCollection;
        }

        private static (ICollection<Cookie>, Dictionary<string, string>)
            GetCookiesAndSession(HttpHeaderCollection httpHeaderCollection)
        {
            var cookiesAsString =
                httpHeaderCollection.FirstOrDefault
                    (x => x.Name == HttpConstants.RequestCookieHeader).Value;

            var cookies = new List<Cookie>();

            if (cookiesAsString != null)
            {
                var splitCookies = cookiesAsString.Split(new string[] { "; " },
                    StringSplitOptions.RemoveEmptyEntries);

                foreach (var cookieAsString in splitCookies)
                {
                    cookies.Add(new Cookie(cookieAsString));
                }
            }

            var session = GetSession(ref cookies);
            return (cookies, session);
        }

        private static Dictionary<string, string> GetSession
            (ref List<Cookie> cookies)
        {
            var sessionCookie = cookies
                .FirstOrDefault(x => x.Name == HttpConstants.SessionCookieName);

            var session = new Dictionary<string, string>();
            // Session is created empty here
            // but later on in the User Controller we will use it
            // to log in or log out the user depending on the data the Session contains

            if (sessionCookie == null)
            {
                var sessionId = Guid.NewGuid().ToString();
                Sessions.Add(sessionId, session);
                cookies.Add(new Cookie(HttpConstants.SessionCookieName, sessionId));
            }
            else if (!Sessions.ContainsKey(sessionCookie.Value))
            {
                Sessions.Add(sessionCookie.Value, session);
                // This code keeps the current session "alive"
            }
            else
            {
                session = Sessions[sessionCookie.Value];
            }

            return session;
        }
    }
}
