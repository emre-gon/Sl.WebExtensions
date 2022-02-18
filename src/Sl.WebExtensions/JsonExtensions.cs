using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Sl.WebExtensions
{
    public static class JsonExtensions
    {
        public static string ToJason(this object Object)
        {
            return JsonConvert.SerializeObject(Object, new JsonSerializerSettings()
            {
                Culture = CultureInfo.CurrentCulture
            });
        }

        private static T GetDefaultGeneric<T>() => default(T);
        public static object GetDefault(Type t)
            => typeof(JsonExtensions).GetMethod("GetDefaultGeneric").MakeGenericMethod(t).Invoke(null, null);



        public static T ConvertObjectTo<T>(this object Object)
        {
            return (T)Object.ConvertObjectTo(typeof(T));
        }

        public static object ConvertObjectTo(this object Object, Type ResultType)
        {
            if (Object == null)
                return GetDefault(ResultType);

            if (Object.GetType() == ResultType)
                return Object;

            try
            {
                return Object.ToJason().ParseJson(ResultType);
            }
            catch (Exception)
            {
                throw new Exception($"Object: '{Object}' could not be parsed into type: {ResultType.FullName}");
            }
        }

        public static object ParseJson(this string String, Type Type)
            => JsonConvert.DeserializeObject(String, Type, new JsonSerializerSettings()
            {
                Culture = CultureInfo.CurrentCulture
            });

        public static T ParseJson<T>(this string String)
        {
            if (String == null)
                return default(T);

            return JsonConvert.DeserializeObject<T>(String, new JsonSerializerSettings()
            {
                Culture = CultureInfo.CurrentCulture
            });
        }

        public static void SetPropertyValues(this object obj, IDictionary<string, object> PropertyValueDict)
        {
            var currentObject = obj;
            foreach (var Key in PropertyValueDict.Keys)
            {
                string[] splitted = Key.Split('.');

                for (int i = 0; i < splitted.Length - 1; i++)
                {
                    var fProp = currentObject.GetType().GetProperty(splitted[i]);
                    if (fProp.GetValue(currentObject) == null && PropertyValueDict[Key] != null)
                    {
                        //creates a default instance for foreign object
                        fProp.SetValue(currentObject, Activator.CreateInstance(fProp.PropertyType));
                    }

                    currentObject = fProp.GetValue(currentObject);
                }

                var propToSet = currentObject.GetType().GetProperty(splitted[splitted.Length - 1]);
                var value = PropertyValueDict[Key].ConvertObjectTo(propToSet.PropertyType);
                propToSet.SetValue(currentObject, value);
                currentObject = obj;
            }
        }

    }
}
