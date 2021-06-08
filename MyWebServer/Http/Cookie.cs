namespace MyWebServer.Http
{
    using MyWebServer.Common;

    public class Cookie
    {
        public Cookie(string name, string value)
        {
            Guard.AgainstNull(name, nameof(name));
            Guard.AgainstNull(value, nameof(value));

            this.Name = name;
            this.Value = value;
        }

        public Cookie(string cookieAsString)
        {
            Guard.AgainstNull(cookieAsString, nameof(cookieAsString));

            var cookieParts = cookieAsString.Split(new [] { '=' }, 2);
            this.Name = cookieParts[0];
            this.Value = cookieParts[1];
        }

        public string Name { get; set; }

        public string Value { get; set; }

        public override string ToString()
        {
            return $"{this.Name}={this.Value}";
        }
    }
}
