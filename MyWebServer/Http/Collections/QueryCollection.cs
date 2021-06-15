namespace MyWebServer.Http.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class QueryCollection : IEnumerable<string>
    {
        private readonly Dictionary<string, string> query;

        public QueryCollection()
            => this.query = new(StringComparer.InvariantCultureIgnoreCase);

        public string this[string name]
            => this.query[name];

        public void Add(string name, string value)
            => this.query[name] = value;

        public bool Contains(string name)
            => this.query.ContainsKey(name);

        public string GetValueOrDefault(string key)
             => this.query.GetValueOrDefault(key);

        public IEnumerator<string> GetEnumerator()
            => query.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}
