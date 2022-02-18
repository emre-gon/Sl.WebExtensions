using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sl.WebExtensions.ValidationExtensions
{
    public class TCKimlikNoAttribute : DataTypeAttribute, IClientModelValidator
    {
        public TCKimlikNoAttribute()
            : base("Tckn")
        {

        }

        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;


            var valueStr = value.ToString();

            if (string.IsNullOrEmpty(valueStr))
                return ValidationResult.Success;

            if(valueStr.Length != 11)
                return new ValidationResult(GetErrorMessage());

            int[] values = new int[11];


            int teklerToplam = 0;
            int ciftlerToplam = 0;
            int toplam = 0;
            for(int i = 0; i < 9; i++)
            {
                if (int.TryParse(valueStr[0].ToString(), out int val))
                {
                    if(i == 0 && val == 0)
                    {
                        //ilk hane 0 olamaz
                        return new ValidationResult(GetErrorMessage());
                    }


                    if(i % 2 == 0)
                    {
                        teklerToplam += val;
                    }
                    else
                    {
                        ciftlerToplam += val;
                    }

                    toplam += val;

                }
                else
                    return new ValidationResult(GetErrorMessage());
            }

            

            if(!int.TryParse(valueStr[9].ToString(), out int onuncuBasamak))
            {
                return new ValidationResult(GetErrorMessage());
            }

            if (!int.TryParse(valueStr[10].ToString(), out int onbirinciBasamak))
            {
                return new ValidationResult(GetErrorMessage());
            }



            toplam += onuncuBasamak;

            if(toplam % 10 != onbirinciBasamak)
            {
                return new ValidationResult(GetErrorMessage());
            }



            if ((teklerToplam * 7 - ciftlerToplam) % 10 != onuncuBasamak)
            {
                return new ValidationResult(GetErrorMessage());
            }


            return ValidationResult.Success;
        }


        public string GetErrorMessage()
        {
            return $"Geçerli bir TC Kimlik Numarası giriniz.";
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val-tckn", GetErrorMessage());
            context.Attributes.Add("maxlength", "11");
        }


    }
}
