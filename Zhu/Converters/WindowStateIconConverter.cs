using MaterialDesignThemes.Wpf;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Zhu.Converters
{
    [ValueConversion(typeof(WindowState), typeof(PackIconKind))]
    public class WindowStateIconConverter : MarkupExtension, IValueConverter
    {
        private WindowStateIconConverter _instance;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isFullscreen = (bool)value; 
            return isFullscreen ? PackIconKind.FullscreenExit : PackIconKind.Fullscreen;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
            => _instance ?? (_instance = new WindowStateIconConverter());
    }
}
