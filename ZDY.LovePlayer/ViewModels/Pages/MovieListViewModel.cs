using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using NLog;
using ZDY.LovePlayer.Messaging;
using ZDY.LovePlayer.Services;
using ZDY.LovePlayer.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using ZDY.LovePlayer.Untils;
using System.Collections.Generic;
using ZDY.LovePlayer.Controls;
using MaterialDesignThemes.Wpf;
using System.Threading;
using GalaSoft.MvvmLight.Ioc;
using Infrastructure.SearchModel.Model;
using ZDY.LovePlayer.UserControls.Reused;

namespace ZDY.LovePlayer.ViewModels.Pages
{
    public class MovieListViewModel : MediaListViewModel
    {
        #region Constructor

        public MovieListViewModel(IApplicationState applicationState, IMediaService mediaService)
            : base(applicationState, mediaService)
        {

        }

        #endregion

        #region Search Options

        public Dictionary<string, string> SortFields { get; set; }
        public List<string> CountryCollection { get; set; }
        public List<string> MediaLengthCollection { get; set; }
        public List<string> ImageQualityCollection { get; set; }

        public void InitializeSearchOptions()
        {
            SortFields = new Dictionary<string, string>();
            SortFields.Add("加入顺序", "ID");
            SortFields.Add("发布时间", "PubDate");
            SortFields.Add("影片编码", "Code");

            CountryCollection = new List<string> { "不限", "内地", "港台", "日本", "欧美", "韩国", "其他" };
            MediaLengthCollection = new List<string> { "不限", "0-10分钟", "10-30分钟", "30-60分钟", "60分钟以上" };
            ImageQualityCollection = new List<string> { "不限", "高清", "超清", "1080P", "4K" };
        }

        #endregion

        #region Properties

        private string _selectCountry;
        public string SelectCountry
        {
            get { return _selectCountry; }
            set { Set(() => SelectCountry, ref _selectCountry, value); }
        }

        private string _selectMediaLength;
        public string SelectMediaLength
        {
            get { return _selectMediaLength; }
            set { Set(() => SelectMediaLength, ref _selectMediaLength, value); }
        }

        private string _selectImageQuality;
        public string SelectImageQuality
        {
            get { return _selectImageQuality; }
            set { Set(() => SelectImageQuality, ref _selectImageQuality, value); }
        }

        #endregion

        #region Methods

        public override void ExecuteLoadMedias(bool isRefresh = false)
        {
            if (!this.SearchQueryModel.Items.Any(t => t.Field == "MediaType"))
            {
                this.SearchQueryModel.Items.Add(new ConditionItem("MediaType", QueryMethod.LessThanOrEqual, (int)PubilcEnum.MediaKind.Movie));
            }
            if (!string.IsNullOrEmpty(SelectCountry) && SelectCountry != "不限")
            {
                this.SearchQueryModel.Items.Add(new ConditionItem("Countries", QueryMethod.Equal, SelectCountry));
            }
            base.ExecuteLoadMedias(isRefresh);
        }

        #endregion
    }
}
