using System;
using System.Globalization;
using System.Windows.Data;

namespace ZdyLovePlayer.Converters
{
    public class MediaListItemWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            double width = 0;
            if (value != null && value.Length == 3)
            {
                double containerWidth = System.Convert.ToDouble(value[0], CultureInfo.InvariantCulture);
                double itemWidth = System.Convert.ToDouble(value[1], CultureInfo.InvariantCulture);
                double itemOffset = System.Convert.ToDouble(value[2], CultureInfo.InvariantCulture);

                if ((containerWidth - itemOffset) % (itemWidth + itemOffset) == 0)
                {
                    width = itemWidth;
                }
                else
                {
                    width = (containerWidth - itemOffset) / Math.Floor((containerWidth - itemOffset) / (itemWidth + itemOffset)) - itemOffset;
                }
            }
            return width;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
