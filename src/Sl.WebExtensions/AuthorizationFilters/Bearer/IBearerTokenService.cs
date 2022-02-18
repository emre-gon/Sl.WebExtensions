using System;
using System.Collections.Generic;
using System.Text;

namespace Sl.WebExtensions.AuthorizationFilters.Bearer
{
    public interface IBearerTokenService
    {
        bool IsValidToken(string realm, string bearerToken);
    }
}
