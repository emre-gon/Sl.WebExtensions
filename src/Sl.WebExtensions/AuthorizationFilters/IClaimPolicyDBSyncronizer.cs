using System;
using System.Collections.Generic;
using System.Text;

namespace Sl.WebExtensions.AuthorizationFilters
{
    public interface IClaimPolicyDBSyncronizer
    {
        void SyncClaimPoliciesWithDB(ISet<string> ClaimPoliciesFromControllers);
    }
}
