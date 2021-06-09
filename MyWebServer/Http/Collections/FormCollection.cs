namespace MyWebServer.Http.Collections
{
    using System.Collections;
    using System.Collections.Generic;

    public class FormCollection : IEnumerable<string>
    {
        private readonly Dictionary<string, string> form;

        public FormCollection(Dictionary<string, string> form)
            => this.form = form;
        public FormCollection() 
            :this(new Dictionary<string, string>())
        {
        }

        public string this[string value]
            => this.form[value];

        public void Add(string name, string value) 
            => this.form.Add(name, value);

        public bool Contains(string name)
            => this.form.ContainsKey(name);

        public IEnumerator<string> GetEnumerator() 
            => form.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => this.GetEnumerator();
    }
}
