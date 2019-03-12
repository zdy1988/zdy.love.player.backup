using MaterialDesignThemes.Wpf;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ZdyLovePlayer.Converters
{
    [ValueConversion(typeof(WindowState), typeof(PackIconKind))]
    public class WindowStateIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isFullscreen = (bool)value; 
            return isFullscreen ? PackIconKind.FullscreenExit : PackIconKind.Fullscreen;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
