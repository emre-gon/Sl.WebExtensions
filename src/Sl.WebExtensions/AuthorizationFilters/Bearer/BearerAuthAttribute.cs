using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Sl.WebExtensions.AuthorizationFilters.Bearer
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class BearerAuthAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _realm;

        public BearerAuthAttribute(string realm = "Default")
        {
            _realm = realm;
            if (string.IsNullOrWhiteSpace(_realm))
            {
                throw new ArgumentNullException(nameof(realm), @"Realm cannot be empty.");
            }
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                string authHeader = context.HttpContext.Request.Headers["Authorization"];
                if (authHeader != null)
                {
                    var authHeaderValue = AuthenticationHeaderValue.Parse(authHeader);
                    if (authHeaderValue.Scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase))
                    {
                        string value = authHeaderValue.Parameter;

                        if (IsAuthorized(context, _realm, value))
                        {
                            return;
                        }

                    }
                }

                context.Result = new UnauthorizedResult();
            }
            catch (FormatException)
            {
                context.Result = new UnauthorizedResult();
            }
        }

        public bool IsAuthorized(AuthorizationFilterContext context, string realm, string bearerToken)
        {
            var userService = context.HttpContext.RequestServices.GetRequiredService<IBearerTokenService>();
            return userService.IsValidToken(realm, bearerToken);
        }
    }
}
