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
    public class UriToCachedImageConverter : MarkupExtension, IValueConverter
    {
        private UriToCachedImageConverter _instance;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return value;
            }
            else
            {
                return Untils.Constants.MovieCoverDictionary + value.ToString();
            }

            if (string.IsNullOrEmpty(value?.ToString()))
            {
                return Untils.BitmapImageHelper.GetBitmapImage("/Resources/Images/ProfilePic.jpg");
            }

            var path = value.ToString();
            if (path.StartsWith("http"))
            {
                var bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(path);
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.EndInit();
                return bi;
            }
            else
            {
                path = Untils.Constants.MovieCoverDictionary + path;
                var byteArray = Untils.BitmapImageHelper.BitmapImageToByteArray(path);
                if (byteArray != null)
                {
                    return Untils.BitmapImageHelper.ByteArrayToBitmapImage(byteArray);
                }
                else
                {
                    return Untils.BitmapImageHelper.GetBitmapImage("/Resources/Contact.png");
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
            => _instance ?? (_instance = new UriToCachedImageConverter());
    }
}
