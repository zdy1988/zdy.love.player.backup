using MaterialDesignColors;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Zhu.Converters
{
    [ValueConversion(typeof(int), typeof(System.Windows.Media.Color))]
    public class RandomColorSwatchesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var Swatches = new SwatchesProvider().Swatches.ToList();
            Random ran = new Random();
            int i = ran.Next(0, Swatches.Count());
            return Swatches[i].ExemplarHue.Color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
