namespace MyWebServer.Http.Collections
{
    using System.Collections;
    using System.Collections.Generic;

    public class HttpHeaderCollection : IEnumerable<HttpHeader>
    {
        private readonly Dictionary<string, HttpHeader> headers;

        public HttpHeaderCollection()
            => this.headers = new Dictionary<string, HttpHeader>();

        public string this[string name] 
            => this.headers[name].Value;

        public int Count => this.headers.Count;

        public void Add(string name, string value)
        {
            var header = new HttpHeader(name, value);

            this.headers.Add(name, header);
        }

        public bool Contains(string name) 
            => this.headers.ContainsKey(name);

        public IEnumerator<HttpHeader> GetEnumerator()
            => this.headers.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}