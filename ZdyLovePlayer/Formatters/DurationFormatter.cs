using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ZdyLovePlayer.Formatters
{
    /// <summary>
    /// UI-friendly duration formatter.
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class DurationFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var duration = value is TimeSpan ? (TimeSpan)value : TimeSpan.FromSeconds(-1);

            if (duration.TotalSeconds <= 0)
                return "∞";

            if (duration.TotalMinutes >= 100)
            {
                return $"{System.Convert.ToInt64(duration.TotalHours)}h {System.Convert.ToInt64(duration.Minutes)}m";
            }

            return $"{System.Convert.ToInt64(duration.Minutes):00}:{System.Convert.ToInt64(duration.Seconds):00}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
