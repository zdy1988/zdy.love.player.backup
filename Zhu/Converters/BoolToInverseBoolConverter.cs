using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Zhu.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class BoolToInverseBoolConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture) => !(bool)value;

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private BoolToInverseBoolConverter _instance;

        public override object ProvideValue(IServiceProvider serviceProvider)
            => _instance ?? (_instance = new BoolToInverseBoolConverter());
    }
}