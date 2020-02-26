using MaterialDesignThemes.Wpf;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ZDY.LovePlayer.Converters
{
    [ValueConversion(typeof(WindowState), typeof(string))]
    public class WindowStateTooltipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isFullscreen = (bool)value; 
            return isFullscreen ? "还原" : "最大化";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
