namespace MyWebServer.Demo.Data
{
    using System.Collections.Generic;
    using MyWebServer.Demo.Data.Models;

    public interface IData
    {
        IEnumerable<Cat> Cats { get; }
    }
}
