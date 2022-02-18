using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sl.WebExtensions.AuthorizationFilters
{
    public static class AuthPolicyExtensions
    {
        public static IEnumerable<string> GetClaimPolicies<TController, TClaimsDBSyncronizer>()
            where TController : ControllerBase
            where TClaimsDBSyncronizer : IClaimPolicyDBSyncronizer, new()
        {
            return GetClaimPolicies<TController>(new TClaimsDBSyncronizer());
        }


        public static IEnumerable<string> GetClaimPolicies<TController>()
            where TController : ControllerBase
        {
            return GetClaimPolicies<TController>(null);
        }

        private static IEnumerable<string> GetClaimPolicies<T>(IClaimPolicyDBSyncronizer syncronizer)
            where T : ControllerBase
        {
            #region get claims in controllers
            var controllerTypes = Assembly.GetAssembly(typeof(T))
                .GetTypes().Where(f => f.IsClass && f.IsSubclassOf(typeof(ControllerBase)));

            var methods = controllerTypes.SelectMany(f => f.GetMethods().Where(q => q.IsDefined(typeof(AuthorizeAttribute))));


            var methodClaims = methods.SelectMany(f => f.GetCustomAttributes<AuthorizeAttribute>())
                .Where(f => !string.IsNullOrEmpty(f.Policy))
                .Select(f => f.Policy);

            var controllerClaims = controllerTypes.SelectMany(f => f.GetCustomAttributes<AuthorizeAttribute>())
                .Where(f => !string.IsNullOrEmpty(f.Policy))
                .Select(f => f.Policy);

            var claimPolicies = methodClaims.Union(controllerClaims).ToHashSet();
            #endregion

            if (syncronizer != null)
            {
                syncronizer.SyncClaimPoliciesWithDB(claimPolicies);
            }

            return claimPolicies;
            
        }
    }
}
