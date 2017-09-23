using System;
using System.Globalization;
using System.Windows.Controls;

namespace Zhu.ValidationRules
{
    public class IsUriValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrWhiteSpace((value ?? "").ToString()))
            {
                return ValidationResult.ValidResult;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(value.ToString(), @"http(s)?://([\w-]+\.)+[\w-]+(/[\w-+ ./?%&=]*)?"))
            {
                 return new ValidationResult(false, "网络媒体地址不合法！");
            }

            return ValidationResult.ValidResult;
        }
    }
}
