namespace MyWebServer.Http.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class CookieCollection : IEnumerable<HttpCookie>
    {
        private readonly Dictionary<string, HttpCookie> cookies;

        public CookieCollection()
            => this.cookies = new(StringComparer.InvariantCultureIgnoreCase);

        public string this[string name]
            => this.cookies[name].Value;

        public int Count => this.cookies.Count;

        public void Add(string name, string value)  
            => this.cookies[name] = new HttpCookie(name, value);

        public bool Contains(string name)
            => this.cookies.ContainsKey(name);

        public IEnumerator<HttpCookie> GetEnumerator()
            => this.cookies.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}