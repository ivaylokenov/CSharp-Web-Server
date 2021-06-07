namespace MyWebServer.Http
{
    using MyWebServer.Common;

    public class HttpHeader
    {
        public HttpHeader(string name, string value)
        {
            Guard.AgainstNull(name, nameof(name));
            Guard.AgainstNull(value, nameof(value));

            this.Name = name;
            this.Value = value;
        }

        public string Name { get; init; }

        public string Value { get; init; }

        public override string ToString()
            => $"{this.Name}: {this.Value}";
    }
}
