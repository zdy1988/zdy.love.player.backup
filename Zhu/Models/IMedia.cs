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
        /// 媒体地址
        /// </summary>
        string Path { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        string SubType { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        string Title { get; set; }
        /// <summary>
        /// 语言
        /// </summary>
        string Languages { get; set; }
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
        /// 是否最喜欢的
        /// </summary>
        Boolean? IsFavorite { get; set; }
        /// <summary>
        /// 评分
        /// </summary>
        int? Rating { get; set; }
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
