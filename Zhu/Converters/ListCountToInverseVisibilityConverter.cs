using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Zhu.Converters
{
    public class ListCountToInverseVisibilityConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("The target must be a VisibilityProperty");

            if ((((ICollection)value).Count > 0))
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private ListCountToInverseVisibilityConverter _instance;

        public override object ProvideValue(IServiceProvider serviceProvider)
            => _instance ?? (_instance = new ListCountToInverseVisibilityConverter());
    }
}
