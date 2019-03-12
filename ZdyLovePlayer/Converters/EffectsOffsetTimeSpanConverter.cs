using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace ZdyLovePlayer.Converters
{
    [ValueConversion(typeof(string), typeof(TimeSpan))]
    public class EffectsOffsetTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan unit = new TimeSpan(0, 0, 0, 0, 25);
            var i = int.Parse(value.ToString());
            return new TimeSpan(unit.Ticks * i);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
