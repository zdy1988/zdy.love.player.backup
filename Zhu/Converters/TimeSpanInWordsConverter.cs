using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace Zhu.Converters
{
    public class TimeSpanInWordsConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sapn = new TimeSpan((long)value);
            return Untils.TimeSpanHelper.TimeSpanInWords(sapn);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private TimeSpanInWordsConverter _instance;

        public override object ProvideValue(IServiceProvider serviceProvider)
            => _instance ?? (_instance = new TimeSpanInWordsConverter());
    }
}
