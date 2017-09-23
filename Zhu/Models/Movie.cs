using GalaSoft.MvvmLight.Ioc;
using Infrastructure;
using Zhu.Services;
using System;

namespace Zhu.Models
{
    public class Movie : Media
    {
        /// <summary>
        /// MD5值
        /// </summary>
        public string MD5 { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 在线地址
        /// </summary>
        public string StreamNetworkAddress { get; set; }
        /// <summary>
        /// 导演
        /// </summary>
        public string Directors { get; set; }
        /// <summary>
        /// 上映时间
        /// </summary>
        public DateTime? Pubdates { get; set; }
        /// <summary>
        /// 年代
        /// </summary>
        public int? Year { get; set; }
        /// <summary>
        /// 片长
        /// </summary>
        public string Durations { get; set; }




        public override string Path
        {
            get
            {
                return string.IsNullOrEmpty(this.FilePath) ? this.StreamNetworkAddress : this.FilePath;
            }
        }
    }
}
