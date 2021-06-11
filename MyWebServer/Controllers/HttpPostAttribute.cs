namespace MyWebServer.Controllers
{
    using MyWebServer.Http;

    public class HttpPostAttribute : HttpMethodAttribute
    {
        public HttpPostAttribute()
            : base(HttpMethod.Post)
        {
        }
    }
}
