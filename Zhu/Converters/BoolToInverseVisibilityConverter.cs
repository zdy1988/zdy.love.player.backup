using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Zhu.Converters
{
    public class BoolToInverseVisibilityConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("The target must be a VisibilityProperty");

            if ((bool)value)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private BoolToInverseVisibilityConverter _instance;

        public override object ProvideValue(IServiceProvider serviceProvider)
            => _instance ?? (_instance = new BoolToInverseVisibilityConverter());
    }
}
