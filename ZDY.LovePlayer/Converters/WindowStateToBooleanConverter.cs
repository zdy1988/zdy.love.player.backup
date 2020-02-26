using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ZDY.LovePlayer.Converters
{
    /// <summary>
    /// Used to convert a window state to a boolean
    /// </summary>
    [ValueConversion(typeof(WindowState), typeof(bool))]
    public class WindowStateToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Convert boolean to a window state
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>True if maximized, false otherwise</returns>
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            var isFullscreen = (bool)value;
            return isFullscreen ? WindowState.Maximized : WindowState.Normal;
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            var windowState = (WindowState)value;
            return windowState != WindowState.Minimized && windowState != WindowState.Normal;
        }
    }
}
