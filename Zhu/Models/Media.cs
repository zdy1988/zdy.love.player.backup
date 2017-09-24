using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhu.Models
{
    public class Media : IEntity
    {   
        public int ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int MediaType { get; set; }
        /// <summary>
        /// MD5
        /// </summary>
        public string MD5 { get; set; }
        /// <summary>
        /// 封面
        /// </summary>
        public string Cover { get; set; }
        /// <summary>
        /// 媒体编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 视频源
        /// </summary>
        public string MediaSource { get; set; }
        /// <summary>
        /// 视频源类型
        /// </summary>
        public int MediaSourceType { get; set; }
        /// <summary>
        /// 表演者
        /// </summary>
        public string Actors { get; set; }
        /// <summary>
        /// 导演
        /// </summary>
        public string Directors { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? PubDate { get; set; }
        /// <summary>
        /// 年代
        /// </summary>
        public int? Year { get; set; }
        /// <summary>
        /// 语言
        /// </summary>
        public string Languages { get; set; }
        /// <summary>
        /// 时长
        /// </summary>
        public int? Durations { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        public string Countries { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 介绍
        /// </summary>
        public string Introduction { get; set; }
        /// <summary>
        /// 评分
        /// </summary>
        public int? Rating { get; set; }
        /// <summary>
        /// 是否最喜欢的
        /// </summary>
        public Boolean? IsFavorite { get; set; }
        /// <summary>
        /// 录入时间
        /// </summary>
        public DateTime? EnterDate { get; set; }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? UpdateDate { get; set; }
    }
}
