namespace MyWebServer.App.Data
{
    using System.Collections.Generic;
    using MyWebServer.App.Data.Models;

    public interface IData
    {
        IEnumerable<Cat> Cats { get; }
    }
}
