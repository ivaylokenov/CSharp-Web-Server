namespace MyWebServer.Services
{
    public interface IServiceCollection
    {
        IServiceCollection Add<TService, TImplementation>()
            where TService : class
            where TImplementation : TService;

        IServiceCollection Add<TService>()
            where TService : class;
    }
}
