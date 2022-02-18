using System;
using System.Collections.Generic;
using System.Text;

namespace Sl.WebExtensions.AuthorizationFilters.Basic
{
    public interface IUserService
    {
        bool IsValidUser(string username, string password);
    }
}
