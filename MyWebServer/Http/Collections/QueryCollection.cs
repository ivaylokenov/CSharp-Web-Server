namespace MyWebServer.Http.Collections
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class QueryCollection : IEnumerable<string>
    {
        private readonly Dictionary<string, string> query;
        public QueryCollection(Dictionary<string, string> query)
            => this.query = query.ToDictionary(p => p.Key.ToLower(), p => p.Value);
        public QueryCollection()
            : this(new Dictionary<string, string>())
        {
        }

        public string this[string value]
            => this.query[value.ToLower()];

        public void Add(string name, string value)
            => this.query.Add(name, value);

        public bool Contains(string name)
            => this.query.ContainsKey(name.ToLower()); 

        public IEnumerator<string> GetEnumerator()
            => query.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}
