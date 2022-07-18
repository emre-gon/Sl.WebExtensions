using Microsoft.AspNetCore.Http;
using Sl.JsonExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sl.WebExtensions
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, value.ToJason());
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) : value.ParseJson<T>();
        }
    }
}
