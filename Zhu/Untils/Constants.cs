using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhu.Untils
{
    public static class Constants
    {
        /// <summary>
        /// 程序名称
        /// </summary>
        public const string ApplicationName = "Zhu Player";

        /// <summary>
        /// 资源目录
        /// </summary>
        public static string TempDictionary { get; } = Path.GetTempPath() + "Zhu\\Assets\\";
        //public static string Assets { get; } = Path.GetTempPath() + "Popcorn\\Assets\\";

        //F:\照片  E:\vedio
        public const string DictionaryRoot = @"E:\vedio";

        /// <summary>
        /// 影片封面文件夹
        /// </summary>
        public const string MovieCoverDictionary = @"D:\\MovieImage\\Movies\\";

        /// <summary>
        /// 演员封面文件夹
        /// </summary>
        public const string ActorCoverDictionary = @"D:\\MovieImage\\Actors\\";

        /// <summary>
        /// 加载影片数据条数
        /// </summary>
        public static int LoadDataPageSize { get; } = 20;

        /// <summary>
        /// 支持的视频格式
        /// </summary>
        public static string VedioFilters = "(可用视频格式)|*.asf;*.avi;*.divx;*.dv;*.flv;*.gxf;*.m1v;*.m2v;*.m2ts;*.m4v;*.mkv;*.mov;*.mp2;*.mp4;*.mpeg;*.mpeg1;*.mpeg2;*.mpeg4;*.mpg;*.mts;*.mxf;*.ogg;*.ogm;*.ps;*.ts;*.vob;*.wmv;*.a52;*.aac;*.ac3;*.dts;*.flac;*.m4a;*.m4p;*.mka;*.mod;*.mp1;*.mp2;*.mp3;*.ogg";

        /// <summary>
        /// 文本格式
        /// </summary>
        public static string TxtFilters = "(可用视频格式)|*.txt";
    }
}
