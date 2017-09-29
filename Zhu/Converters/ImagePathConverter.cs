using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace Zhu.Converters
{
    public class ImagePathConverter : MarkupExtension, IValueConverter
    {
        private ImagePathConverter _instance;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return null;
            }

            return $"{Untils.Constants.ImageSourcePath}{parameter.ToString()}\\{value.ToString()}";
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
            => _instance ?? (_instance = new ImagePathConverter());
    }
}
