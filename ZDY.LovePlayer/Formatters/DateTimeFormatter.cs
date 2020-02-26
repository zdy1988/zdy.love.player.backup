using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace ZDY.LovePlayer.Formatters
{
    public class DateTimeFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            string dateFormat = "yyyy/MM/dd hh:ss";
            if (parameter != null)
            {
                dateFormat = parameter.ToString();
            }
            return date.ToString(dateFormat);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
