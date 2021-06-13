namespace MyWebServer.Controllers
{
    using System;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AuthorizeAttribute : Attribute
    {
    }
}
