using System.Globalization;
using System.Windows.Controls;

namespace Zhu.ValidationRules 
{
    public class FileExistsValidationRule: ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrWhiteSpace((value ?? "").ToString()))
            {
                return ValidationResult.ValidResult;
            }

            if (!System.IO.File.Exists(value.ToString()))
            {
                return new ValidationResult(false, "未找到媒体文件！");
            }

            return ValidationResult.ValidResult;
        }
    }
}
