namespace MyWebServer.Http.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class QueryCollection : IEnumerable<string>
    {
        private readonly Dictionary<string, string> query;
        public QueryCollection(Dictionary<string, string> query)
            => this.query = query; //.ToDictionary(p => p.Key.ToLower(), p => p.Value);
        public QueryCollection()
            : this(new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase))
        {
        }

        public string this[string value]
            => this.query[value]; //value had toLower()

        public void Add(string name, string value)
            => this.query[name] = value;

        public bool Contains(string name)
            => this.query.ContainsKey(name); // same here 

        public string GetValueOrDefault(string key)
             => this.query.GetValueOrDefault(key);

        public IEnumerator<string> GetEnumerator()
            => query.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}
