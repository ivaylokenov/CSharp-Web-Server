namespace MyWebServer.Http.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class FormCollection : IEnumerable<string>
    {
        private readonly Dictionary<string, string> form;

        public FormCollection()
            => this.form = new(StringComparer.InvariantCultureIgnoreCase);

        public string this[string name]
            => this.form[name];

        public void Add(string name, string value)
            => this.form[name] = value;

        public bool Contains(string name)
            => this.form.ContainsKey(name);

        public string GetValueOrDefault(string key)
             => this.form.GetValueOrDefault(key);

        public IEnumerator<string> GetEnumerator()
            => form.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}
