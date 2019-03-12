using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Unosquare.FFME;

namespace ZdyLovePlayer.Formatters
{
    internal class TimeSpanFormatter : IValueConverter
    {
        /// <summary>
        /// Converts the specified position.
        /// </summary>
        /// <param name="value">The position.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The duration.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The object converted</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null)
            {
                var media = parameter as MediaElement;
                parameter = media?.NaturalDuration ?? TimeSpan.Zero;
            }

            var p = TimeSpan.Zero;
            var d = TimeSpan.Zero;

            if (value is TimeSpan span) p = span;
            if (value is Duration duration1)
                p = duration1.HasTimeSpan ? duration1.TimeSpan : TimeSpan.Zero;

            if (parameter != null)
            {
                if (parameter is TimeSpan) d = (TimeSpan)parameter;
                if (parameter is Duration)
                    d = ((Duration)parameter).HasTimeSpan ? ((Duration)parameter).TimeSpan : TimeSpan.Zero;

                if (d == TimeSpan.Zero) return string.Empty;
                p = TimeSpan.FromTicks(d.Ticks - p.Ticks);
            }

            return $"{(int)p.TotalHours:00}:{p.Minutes:00}:{p.Seconds:00}.{p.Milliseconds:000}";
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <exception cref="NotImplementedException">Expected error</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
