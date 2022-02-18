using Sl.WebExtensions.MvcExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Sl.WebExtensions
{
    public static class ModelBaseExtensions
    {
        public static void AutoMapModel(this object Object, object Model)
        {
            if (Model == null)
                return;

            var props = Model.GetType().GetProperties();

            var objType = Object.GetType();
            foreach(var modelProp in props)
            {
                if (modelProp.IsDefined(typeof(IgnoreModelAutoMapAttribute)))
                    continue;

                var objProp = objType.GetProperty(modelProp.Name);

                if (objProp == null)
                    continue;

                if (objProp.IsDefined(typeof(KeyAttribute)))
                    continue; //cannot map key attribute

                var modelValue = modelProp.GetValue(Model);

                if(modelProp.PropertyType != objProp.PropertyType)
                {
                    try
                    {
                        modelValue = modelValue.ConvertObjectTo(objProp.PropertyType);
                    }
                    catch
                    {
                        continue;
                    }
                }                              

                objProp.SetValue(Object, modelValue);
            }

        }



        public static bool IsNew(this object Model)
        {
            if (Model == null)
                return true;

            var keyColumns = Model.GetType().GetProperties().Where(f => f.IsDefined(typeof(KeyAttribute), true));

            foreach (var kCol in keyColumns)
            {
                var value = kCol.GetValue(Model);

                var defaultValue = kCol.PropertyType.IsValueType ? Activator.CreateInstance(kCol.PropertyType) : null;
                if (value == null && defaultValue == null)
                    return true;
                if (value == null || !value.Equals(defaultValue))
                    return false;
            }

            return true;
        }



        public static Dictionary<string, object> GetIDValues(this CrudModelBase Model)
        {
            var props = Model.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly)
                                            .Where(f => f.GetCustomAttributes(typeof(KeyAttribute), false).Any());

            Dictionary<string, object> toBeReturned = new Dictionary<string, object>();
            foreach (var prop in props)
            {
                toBeReturned.Add(prop.Name, prop.GetValue(Model));
            }

            return toBeReturned;
        }
    }




    public abstract class CrudModelBase
    {
        public override string ToString()
        {
            var myType = this.GetType();
            var dnProp = myType.GetProperty("Name");
            if (dnProp == null)
                return $"{myType.Name} | {string.Join(", ", this.GetIDValues().Select(f => f.Key + "=" + f.Value))}";
            else
                return dnProp.GetValue(this)?.ToString();
        }
    }
}
