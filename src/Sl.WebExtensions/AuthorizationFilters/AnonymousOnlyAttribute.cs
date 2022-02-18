using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sl.WebExtensions.AuthorizationFilters
{
    public static class AnonymousOnlyExtension
    {
        public static IServiceCollection AddAnonymousOnlyAttribute(this IServiceCollection builder,
            string DefaultLoggedInPage)
        {
            AnonymousOnlyAttribute.DefaultLoggedInPage = DefaultLoggedInPage;
            return builder;
        }
    }




    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AnonymousOnlyAttribute : Attribute, IAuthorizationFilter
    {
        internal static string DefaultLoggedInPage { get; set; } = "/";
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                if (context.HttpContext.Request.Method == "GET")
                    context.Result = new RedirectResult(DefaultLoggedInPage);
                else
                    context.Result = new ForbidResult();
            }
        }
    }
}
