using GalaSoft.MvvmLight.Ioc;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Unosquare.FFME.Platform;
using Zhu.Foundation;

namespace Zhu.Converters
{
    /// <summary>
    /// Formsts timespan time measures as string with 3-decimal milliseconds
    /// </summary>
    /// <seealso cref="IValueConverter" />
    internal class PlaylistEntryThumbnailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object format, CultureInfo culture)
        {
            var thumbnailFilename = value as string;
            if (thumbnailFilename == null) return default(ImageSource);
            if (GuiContext.Current.IsInDesignTime) return default(ImageSource);

            return ThumbnailGenerator.GetThumbnail(SimpleIoc.Default.GetInstance<ViewModels.Player.MediaPlayerViewModel>().Playlist.ThumbsDirectory, thumbnailFilename);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
