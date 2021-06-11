namespace MyWebServer.Http
{
    public enum HttpStatusCode
    {
        OK = 200,
        Found = 302,
        BadRequest = 400,
        Unauthorized = 401,
        NotFound = 404,
        InternalServerError = 500
    }
}
