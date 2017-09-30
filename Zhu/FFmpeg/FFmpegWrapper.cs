using Saar.FFmpeg.CSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhu.FFmpeg
{
    public class FFmpegWrapper
    {
        public static void GetSnapshot(string videoSource, string outputFileName, int imageWidth = 0, int imageHeight = 0, int outputNumber = 1)
        {
            if (outputNumber < 1)
            {
                throw new ArgumentException("The outputNumber must greater than or equal to 1 !");
            }
            var media = new MediaReader(videoSource);
            var decoder = media.Decoders.OfType<VideoDecoder>().First();
            decoder.OutFormat = new VideoFormat(decoder.InFormat.Width, decoder.InFormat.Height, AVPixelFormat.Bgr24, 4);
            VideoFrame frame = new VideoFrame();
            for (int i = 0; i < outputNumber; i++)
            {
                if (media.NextFrame(frame, decoder.StreamIndex))
                {
                    imageWidth = imageWidth == 0 ? frame.Format.Width : imageWidth;
                    imageHeight = imageHeight == 0 ? frame.Format.Height : imageHeight;

                    Bitmap image = new Bitmap(imageWidth, imageHeight, frame.Format.Strides[0], PixelFormat.Format24bppRgb, frame.Scan0);
                    outputFileName = i > 1 ? $"{outputFileName}.{i}" : outputFileName;
                    image.Save(outputFileName);
                }
            }
        }
    }
}
