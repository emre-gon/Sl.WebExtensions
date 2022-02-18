using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sl.WebExtensions.MvcExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace Sl.WebExtensions.AuthorizationFilters
{
    public static class AuthChecker
    {
        public static bool IsAuthorized(ViewContext viewContext, string ControllerName, string Action)
        {
            var assemblyFinder = viewContext.HttpContext.RequestServices.GetService(typeof(IEntryAssemblyFinder));

            if (assemblyFinder == null)
                return true;

            Assembly entryAssembly = ((IEntryAssemblyFinder)assemblyFinder).FindEntryAssembly();

            var controllerType = entryAssembly.GetTypes()
                .Where(f => f.Name == ControllerName + "Controller"
                        && typeof(Controller).IsAssignableFrom(f))
                .FirstOrDefault();


            if (controllerType == null)
                return true;

            var actionMethod = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.Name == Action)
                .FirstOrDefault();

            if (actionMethod == null)
                return true;


            bool isAuthenticated = viewContext.HttpContext.User.Identity.IsAuthenticated;

            #region anon only check
            var actionAnonAttribute = actionMethod.GetCustomAttribute<AnonymousOnlyAttribute>();

            if (isAuthenticated && actionAnonAttribute != null)
                return false;


            var controllerAnonAttribute = controllerType.GetCustomAttribute<AnonymousOnlyAttribute>();

            if (isAuthenticated && controllerAnonAttribute != null)
                return false;
            #endregion


            var actionAllowAnonAttribute = actionMethod.GetCustomAttribute<AllowAnonymousAttribute>();

            if (!isAuthenticated && actionAllowAnonAttribute != null)
                return true;


            var actionAuthAttribute = actionMethod.GetCustomAttribute<AuthorizeAttribute>();
            var controllerAuthAttribute = controllerType.GetCustomAttribute<AuthorizeAttribute>();

            if (!isAuthenticated && (actionAuthAttribute != null || controllerAuthAttribute != null))
                return false;



            ClaimsIdentity claimsIdentity = viewContext.HttpContext.User.Identity as ClaimsIdentity;



            bool isControllerAuth = IsAuthorized(claimsIdentity, controllerAuthAttribute);

            if (!isControllerAuth)
                return false;

            return IsAuthorized(claimsIdentity, actionAuthAttribute);

        }

        public static bool IsAuthorized(ClaimsIdentity claimsIdentity, AuthorizeAttribute authAttribute)
        {
            if (authAttribute == null)
                return true;


            if (claimsIdentity == null)
                return false;


            HashSet<string> claims = new HashSet<string>();


            string policy = authAttribute.Policy;

            bool hasClaim = true;
            if (!string.IsNullOrEmpty(authAttribute.Policy))
            {
                hasClaim = hasClaim && claimsIdentity.HasClaim(policy, policy);
            }

            if (!string.IsNullOrEmpty(authAttribute.Roles))
            {
                var splitted = authAttribute.Roles.Split(',').Select(f => f.Trim());

                bool hasRole = false;
                foreach (var role in splitted)
                {
                    hasRole = claimsIdentity.HasClaim(ClaimTypes.Role, role);
                    if (hasRole)
                        break;
                }

                hasClaim = hasClaim && hasRole;
            }

            return hasClaim;


        }
    }
}
