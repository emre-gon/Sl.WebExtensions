using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sl.WebExtensions.MvcExtensions
{
    public class CookieMustEnabledMiddleWare
    {
        private readonly RequestDelegate _next;

        public CookieMustEnabledMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if(httpContext.Request.Method == "GET" && httpContext.Request.Cookies["_cme"] != "1")
                httpContext.Response.Cookies.Append("_cme", "1");
            await _next(httpContext);
        }
    }

    public static class CookieMustEnabledMiddleWareExtensions
    {
        public static IApplicationBuilder AddCookieMustEnabled(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CookieMustEnabledMiddleWare>();
        }
    }
}
