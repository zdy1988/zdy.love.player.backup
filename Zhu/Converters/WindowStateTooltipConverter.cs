using MaterialDesignThemes.Wpf;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Zhu.Converters
{
    [ValueConversion(typeof(WindowState), typeof(string))]
    public class WindowStateTooltipConverter : MarkupExtension, IValueConverter
    {
        private WindowStateTooltipConverter _instance;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isFullscreen = (bool)value; 
            return isFullscreen ? "还原" : "最大化";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
            => _instance ?? (_instance = new WindowStateTooltipConverter());
    }
}
