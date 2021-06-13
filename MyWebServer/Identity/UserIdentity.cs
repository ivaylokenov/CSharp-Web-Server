namespace MyWebServer.Identity
{
    public class UserIdentity
    {
        public string Id { get; init; }

        public bool IsAuthenticated => this.Id != null;
    }
}
