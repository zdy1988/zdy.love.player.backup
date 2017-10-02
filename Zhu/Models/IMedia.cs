using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhu.Models
{
    public interface IMedia : IEntity
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Title { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        int MediaType { get; set; }
        /// <summary>
        /// MD5
        /// </summary>
        string MD5 { get; set; }
        /// <summary>
        /// 封面
        /// </summary>
        string Cover { get; set; }
        /// <summary>
        /// 媒体编码
        /// </summary>
        string Code { get; set; }
        /// <summary>
        /// 视频源
        /// </summary>
        string MediaSource { get; set; }
        /// <summary>
        /// 视频源类型
        /// </summary>
        int MediaSourceType { get; set; }
        /// <summary>
        /// 表演者
        /// </summary>
        string Actors { get; set; }
        /// <summary>
        /// 导演
        /// </summary>
        string Directors { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        DateTime? PubDate { get; set; }
        /// <summary>
        /// 年代
        /// </summary>
        int? Year { get; set; }
        /// <summary>
        /// 语言
        /// </summary>
        string Languages { get; set; }
        /// <summary>
        /// 时长
        /// </summary>
        long Duration { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        string Countries { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        string Summary { get; set; }
        /// <summary>
        /// 介绍
        /// </summary>
        string Introduction { get; set; }
        /// <summary>
        /// 评分
        /// </summary>
        int? Rating { get; set; }
        /// <summary>
        /// 是否最喜欢的
        /// </summary>
        Boolean? IsFavorite { get; set; }
        /// <summary>
        /// 录入时间
        /// </summary>
        DateTime? EnterDate { get; set; }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        DateTime? UpdateDate { get; set; }
    }
}
