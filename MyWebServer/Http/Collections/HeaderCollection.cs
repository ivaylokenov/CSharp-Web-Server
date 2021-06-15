namespace MyWebServer.Http.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class HeaderCollection : IEnumerable<HttpHeader>
    {
        private readonly Dictionary<string, HttpHeader> headers;

        public HeaderCollection(Dictionary<string, HttpHeader> headers)
            => this.headers = headers;
        public HeaderCollection()
            : this(new Dictionary<string, HttpHeader>(StringComparer.InvariantCultureIgnoreCase))
        {
        }

        public string this[string name] 
            => this.headers[name].Value;

        public int Count => this.headers.Count;

        public void Add(string name, string value)
        {
            var header = new HttpHeader(name, value);

            this.headers[name] = header;
        }

        public bool Contains(string name) 
            => this.headers.ContainsKey(name);

        public IEnumerator<HttpHeader> GetEnumerator()
            => this.headers.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}