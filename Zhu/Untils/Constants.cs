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
        /// 产品名称
        /// </summary>
        public static string ProductName { get; } = "Zhu Player";

        /// <summary>
        /// 产品Url
        /// </summary>
        public static string ProductUrl { get; } = "";

        /// <summary>
        /// 产品版本
        /// </summary>
        public static string ProductVersion { get; } = "v1.0";

        /// <summary>
        /// 数据库地址
        /// </summary>
        public static string DatabasePath { get; } = Environment.CurrentDirectory + "/ZhuPlayer.db";

        /// <summary>
        /// 图片数据源地址
        /// </summary>
        public static string ImageSourcePath { get; } = @"D:\\MovieImage\\";

        /// <summary>
        /// 图片临时目录
        /// </summary>
        public static string ImageTempDirectory { get; } = Path.GetTempPath() + "ZhuPlayer\\Image\\";
        //public static string ResourceDictionary { get; } = "D:\\MovieImage\\";

        /// <summary>
        /// 加载影片数据条数
        /// </summary>
        public static int LoadDataPageSize { get; } = 25;

        /// <summary>
        /// 支持的视频格式
        /// </summary>
        public static string VedioFilters { get; } = "(可用视频格式)|*.asf;*.avi;*.rmvb;*.divx;*.dv;*.flv;*.gxf;*.m1v;*.m2v;*.m2ts;*.m4v;*.mkv;*.mov;*.mp2;*.mp4;*.mpeg;*.mpeg1;*.mpeg2;*.mpeg4;*.mpg;*.mts;*.mxf;*.ogg;*.ogm;*.ps;*.ts;*.vob;*.wmv;*.a52;*.aac;*.ac3;*.dts;*.flac;*.m4a;*.m4p;*.mka;*.mod;*.mp1;*.mp2;*.mp3;*.ogg;";

        public static bool IsValidVedioFormat(string extension)
        {
            return VedioFilters.IndexOf($"*{extension.ToLower()};") != -1;
        }

        /// <summary>
        /// 文本格式
        /// </summary>
        public static string TxtFilters { get; } = "(可用文件格式)|*.txt;";
    }
}
