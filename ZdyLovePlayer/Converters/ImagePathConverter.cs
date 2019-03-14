using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using ZdyLovePlayer.Models;
using ZdyLovePlayer.PubilcEnum;
using ZdyLovePlayer.Untils;
using ZdyLovePlayer.ViewModels.Pages;

namespace ZdyLovePlayer.Converters
{
    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return "";
            }

            Media media = (Media)value;

            if (media.Cover == null)
            {
                return "";
            }

            // 检测是否是网络地址
            if (NetWorkHelper.IsUrlExist(media.Cover.ToString()))
            {
                return value.ToString();
            }

            // 如果是本地电影，优先从影片的特定目录中加载
            if (media.MediaSourceType == (int)MediaSourceType.LocalFile && parameter.ToString().IndexOf("Movie") != -1)
            {
                if (!File.Exists(media.MediaSource))
                {
                    return "";
                }

                var dictionaryName = Path.GetDirectoryName(media.MediaSource);
                var fileName = Path.GetFileNameWithoutExtension(media.MediaSource);

                var coverPath = $"{dictionaryName}//Cover//{fileName}.jpg";

                if (File.Exists(coverPath))
                {
                    return coverPath;
                }
            }

            // 其他本地文件图片从本地自定目录中获取
            var imagePath = $"{Untils.Constants.ImageSourcePath}{parameter.ToString()}\\{media.Cover.ToString()}";

            if (!File.Exists(imagePath))
            {
                return "";
            }
            else
            {
                return imagePath;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
