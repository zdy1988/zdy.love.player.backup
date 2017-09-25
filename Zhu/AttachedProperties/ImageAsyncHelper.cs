using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Threading;
using NLog;
using Zhu.Untils;

namespace Zhu.AttachedProperties
{
    public enum ImageType
    {
        Thumbnail,
        Poster,
        Backdrop
    }

    public enum ThumbnailSize
    {
        Movie,
        Video,
        NetTV
    }

    public class ImageAsyncHelper : DependencyObject
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        public static ImageType GetImageType(DependencyObject obj)
        {
            return (ImageType)obj.GetValue(ImageTypeProperty);
        }

        public static void SetImageType(DependencyObject obj, ImageType value)
        {
            obj.SetValue(ImageTypeProperty, value);
        }

        public static readonly DependencyProperty ImageTypeProperty =
            DependencyProperty.RegisterAttached("ImageType",
                typeof(ImageType),
                typeof(ImageAsyncHelper),
                new PropertyMetadata(ImageType.Backdrop));

        public static ThumbnailSize GetThumbnailSize(DependencyObject obj)
        {
            return (ThumbnailSize)obj.GetValue(ThumbnailSizeProperty);
        }

        public static void SetThumbnailSize(DependencyObject obj, ThumbnailSize value)
        {
            obj.SetValue(ThumbnailSizeProperty, value);
        }

        public static readonly DependencyProperty ThumbnailSizeProperty =
            DependencyProperty.RegisterAttached("ThumbnailSize",
                typeof(ThumbnailSize),
                typeof(ImageAsyncHelper),
                new PropertyMetadata(ThumbnailSize.Movie));

        public static string GetImagePath(DependencyObject obj)
        {
            return (string)obj.GetValue(ImagePathProperty);
        }

        public static void SetImagePath(DependencyObject obj, Uri value)
        {
            obj.SetValue(ImagePathProperty, value);
        }

        public static readonly DependencyProperty ImagePathProperty = DependencyProperty.RegisterAttached("ImagePath", typeof(string), typeof(ImageAsyncHelper), new PropertyMetadata
        {
            PropertyChangedCallback = async (obj, e) =>
            {
                var image = (Image)obj;
                var imageType = GetImageType(obj);
                var thumbnailSize = GetThumbnailSize(obj);
                var resourceDictionary = new ResourceDictionary
                {
                    Source = new Uri("Zhu;component/Resources/ImageLoading.xaml", UriKind.Relative)
                };

                #region LoadImage

                var loadingImage = resourceDictionary["ImageLoading"] as DrawingImage;
                loadingImage.Freeze();

                #region Create Loading Animation

                var scaleTransform = new ScaleTransform(0.5, 0.5);
                var rotateTransform = new RotateTransform(0);

                var group = new TransformGroup();
                group.Children.Add(scaleTransform);
                group.Children.Add(rotateTransform);

                var doubleAnimation = new DoubleAnimation(0, 359, new TimeSpan(0, 0, 0, 1))
                {
                    RepeatBehavior = RepeatBehavior.Forever
                };

                rotateTransform.BeginAnimation(RotateTransform.AngleProperty, doubleAnimation);

                var loadingAnimationTransform = group;

                #endregion

                image.Source = loadingImage;
                image.Stretch = Stretch.Uniform;
                image.RenderTransformOrigin = new Point(0.5, 0.5);
                image.RenderTransform = loadingAnimationTransform;

                #endregion

                await Task.Run(async () =>
                {
                    try
                    {
                        var path = e.NewValue as string;
                        if (string.IsNullOrEmpty(path))
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                if (imageType == ImageType.Thumbnail)
                                {
                                    var errorThumbnail = resourceDictionary["ImageError"] as DrawingImage;
                                    errorThumbnail.Freeze();
                                    image.RenderTransformOrigin = new Point(0.5d, 0.5d);
                                    image.Stretch = Stretch.None;
                                    image.Source = errorThumbnail;
                                }
                                else
                                {
                                    image.Source = new BitmapImage();
                                }
                            });
                            return;
                        }

                        string localFile;
                        var hash = Convert.ToBase64String(Encoding.UTF8.GetBytes(path));
                        var mustDownload = false;
                        if (File.Exists(Constants.TempDictionary + hash))
                        {
                            localFile = Constants.TempDictionary + hash;
                        }
                        else
                        {
                            mustDownload = true;
                            localFile = Constants.TempDictionary + hash;
                        }

                        try
                        {
                            if (mustDownload)
                            {
                                if (path.StartsWith("http") || path.StartsWith("https"))
                                {
                                    using (var client = new HttpClient())
                                    {
                                        using (var ms = await client.GetStreamAsync(path).ConfigureAwait(false))
                                        {
                                            using (var stream = new MemoryStream())
                                            {
                                                await ms.CopyToAsync(stream).ConfigureAwait(false);
                                                if (!File.Exists(Constants.TempDictionary + hash))
                                                {
                                                    using (var fs = new FileStream(Constants.TempDictionary + hash, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, true))
                                                    {
                                                        var writeAsync = stream.ToArray();
                                                        await fs.WriteAsync(writeAsync, 0, writeAsync.Length).ConfigureAwait(false);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (File.Exists(path))
                                    {
                                        using (var fs = new FileStream(Constants.TempDictionary + hash, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, true))
                                        {
                                            var writeAsync = BitmapImageHelper.BitmapImageToByteArray(path);
                                            await fs.WriteAsync(writeAsync, 0, writeAsync.Length).ConfigureAwait(false);
                                        }
                                    }
                                }
                            }

                            using (var fs = new FileStream(localFile, FileMode.Open, FileAccess.Read))
                            {
                                var bitmapImage = new BitmapImage();
                                bitmapImage.BeginInit();
                                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                if (imageType == ImageType.Thumbnail)
                                {
                                    if (thumbnailSize == ThumbnailSize.Movie)
                                    {
                                        bitmapImage.DecodePixelWidth = 400;
                                        bitmapImage.DecodePixelHeight = 600;
                                    }
                                    else if (thumbnailSize == ThumbnailSize.Video)
                                    {
                                        bitmapImage.DecodePixelWidth = 600;
                                        bitmapImage.DecodePixelHeight = 338;
                                    }
                                    else
                                    {
                                        bitmapImage.DecodePixelWidth = 500;
                                        bitmapImage.DecodePixelHeight = 500;
                                    }
                                }
                                else if (imageType == ImageType.Poster)
                                {
                                    bitmapImage.DecodePixelWidth = 800;
                                    bitmapImage.DecodePixelHeight = 1200;
                                }
                                else
                                {
                                    bitmapImage.DecodePixelWidth = 1920;
                                    bitmapImage.DecodePixelHeight = 1080;
                                }

                                bitmapImage.StreamSource = fs;
                                bitmapImage.EndInit();
                                bitmapImage.Freeze();
                                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                {
                                    image.RenderTransformOrigin = new Point(0, 0);
                                    if (imageType != ImageType.Poster)
                                    {
                                        image.Stretch = Stretch.UniformToFill;
                                    }

                                    image.RenderTransform = new TransformGroup();
                                    RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Unspecified);
                                    image.Source = bitmapImage;
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }

                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            if (imageType == ImageType.Thumbnail)
                            {
                                var errorThumbnail = resourceDictionary["ImageError"] as DrawingImage;
                                errorThumbnail.Freeze();
                                image.RenderTransformOrigin = new Point(0.5d, 0.5d);
                                image.Stretch = Stretch.None;
                                image.Source = errorThumbnail;
                            }
                            else
                            {
                                image.Source = new BitmapImage();
                            }
                        });
                    }
                }).ConfigureAwait(false);
            }
        });
    }
}
