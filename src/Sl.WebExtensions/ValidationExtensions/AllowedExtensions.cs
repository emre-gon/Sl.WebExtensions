using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Sl.WebExtensions.ValidationExtensions
{
    /// <summary>
    /// File extensions
    /// </summary>
    public class AllowedExtensionsAttribute : ValidationAttribute, IClientModelValidator
    {
        public string[] Extensions;

        public AllowedExtensionsAttribute(string CommaSeperatedExtensions)
        {
            Extensions = CommaSeperatedExtensions.Split(',')
                .Select(f => f.Trim().StartsWith(".") ? f.Trim() : "." + f.Trim()).ToArray();
        }


        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;


            IEnumerable<IFormFile> files;
            if(value is IEnumerable<IFormFile>)
            {
                files = (IEnumerable<IFormFile>)value;
            }
            else
            {
                files = new List<IFormFile>()
                {
                    (IFormFile)value
                };
            }


            foreach(var file in files)
            {
                if (!Extensions.Any(f => file.FileName.EndsWith(f)))
                    return new ValidationResult(GetErrorMessage());

            }
            return ValidationResult.Success;
        }


        public string GetErrorMessage()
        {
            return $"Sadece {string.Join(", ", Extensions)} dosya tipleri yüklenebilir.";
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-allowed-extensions-extensions", string.Join(',', Extensions));
            context.Attributes.Add("data-allowed-extensions", GetErrorMessage());
        }


        private bool MergeAttribute(
            IDictionary<string, string> attributes,
            string key,
            string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }
            attributes.Add(key, value);
            return true;
        }
    }
}
