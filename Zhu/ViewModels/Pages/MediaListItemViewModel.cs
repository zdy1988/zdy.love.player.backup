using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhu.Models;
using Zhu.Services;

namespace Zhu.ViewModels.Pages
{
    public class MediaListItemViewModel : ViewModelBase, IMedia
    {
        public int ID { get; set; }

        private string _title;
        /// <summary>
        /// 名称
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                Set(() => Title, ref _title, value);
                ChangeToDataBase();
            }
        }

        private int _mediaType;
        /// <summary>
        /// 类型
        /// </summary>
        public int MediaType
        {
            get { return _mediaType; }
            set
            {
                Set(() => MediaType, ref _mediaType, value);
                ChangeToDataBase();
            }
        }

        private string _md5;
        /// <summary>
        /// MD5
        /// </summary>
        public string MD5
        {
            get { return _md5; }
            set
            {
                Set(() => MD5, ref _md5, value);
                ChangeToDataBase();
            }
        }

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
        public long Duration { get; set; }
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

        private int _rating;
        /// <summary>
        /// 评分
        /// </summary>
        public int Rating
        {
            get { return _rating; }
            set
            {
                Set(() => Rating, ref _rating, value);
                ChangeToDataBase();
            }
        }

        private Boolean _isFavorite;
        /// <summary>
        /// 是否最喜欢的
        /// </summary>
        public Boolean IsFavorite
        {
            get { return _isFavorite; }
            set
            {
                Set(() => IsFavorite, ref _isFavorite, value);
                ChangeToDataBase();
            }
        }

        /// <summary>
        /// 录入时间
        /// </summary>
        public DateTime? EnterDate { get; set; }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? UpdateDate { get; set; }

        private void ChangeToDataBase()
        {
            Task.Run(async () =>
            {
                var _mediaService = SimpleIoc.Default.GetInstance<IMediaService>();
                var media = EmitMapper.ObjectMapperManager.DefaultInstance.GetMapper<MediaListItemViewModel, Media>().Map(this);
                await _mediaService.UpdateAsync(media);
            }).ConfigureAwait(false);
        }
    }
}
