using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sl.WebExtensions.MvcExtensions
{
    public static class HostingEnviromentExtensions
    {
        public static bool IsLocal(this IHostingEnvironment env)
        {
            return env.EnvironmentName == "Local";
        }
    }
}
