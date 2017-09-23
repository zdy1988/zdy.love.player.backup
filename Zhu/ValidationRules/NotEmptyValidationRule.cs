using System.Globalization;
using System.Windows.Controls;

namespace Zhu.ValidationRules
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace((value ?? "").ToString())
                ? new ValidationResult(false, "这个字段是必填项！")
                : ValidationResult.ValidResult;
        }
    }
}
