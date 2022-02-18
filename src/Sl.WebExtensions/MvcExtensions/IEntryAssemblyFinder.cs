using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Sl.WebExtensions.MvcExtensions
{
    public interface IEntryAssemblyFinder
    {
        Assembly FindEntryAssembly();
    }


    public class EntryAssemblyFinder<T> : IEntryAssemblyFinder
        where T : ControllerBase
    {
        public Assembly FindEntryAssembly()
        {
            return Assembly.GetAssembly(typeof(T));
        }
    }
}
