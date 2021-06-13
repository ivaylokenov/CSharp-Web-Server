namespace MyWebServer.Results
{
    using System.Collections.Generic;
    using MyWebServer.Http;

    public abstract class ActionResult : HttpResponse
    {
        protected ActionResult(HttpResponse response) 
            : base(response.StatusCode)
        {
            this.Content = response.Content;

            this.PrepareHeaders(response.Headers);
            this.PrepareCookies(response.Cookies);
        }

        private void PrepareHeaders(IDictionary<string, HttpHeader> headers)
        {
            foreach (var header in headers.Values)
            {
                this.AddHeader(header.Name, header.Value);
            }
        }

        private void PrepareCookies(IDictionary<string, HttpCookie> cookies)
        {
            foreach (var cookie in cookies.Values)
            {
                this.AddCookie(cookie.Name, cookie.Value);
            }
        }
    }
}
