using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using NLog;
using Zhu.Messaging;
using Zhu.Models.ApplicationState;
using Zhu.Services;
using Zhu.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Zhu.Untils;
using System.Collections.Generic;
using Zhu.Controls;
using MaterialDesignThemes.Wpf;
using System.Threading;
using GalaSoft.MvvmLight.Ioc;
using Infrastructure.SearchModel.Model;
using Zhu.UserControls.Reused;

namespace Zhu.ViewModels.Pages
{
    public class MovieListViewModel : MediaListViewModel
    {
        public MovieListViewModel(IApplicationState applicationState, IMediaService mediaService)
            : base(applicationState, mediaService)
        {
            SortFields = new Dictionary<string, string>();
            SortFields.Add("加入顺序", "ID");
            SortFields.Add("发布时间", "PubDate");
            SortFields.Add("影片编码", "Code");
            
            CountryCollection = new List<string> { "不限", "内地", "港台", "日本", "欧美", "韩国", "其他" };
            MediaLengthCollection = new List<string> { "不限", "0-10分钟", "10-30分钟", "30-60分钟", "60分钟以上" };
            ImageQualityCollection = new List<string> { "不限", "高清", "超清", "1080P", "4K" };

            RegisterCommands();
        }

        public Dictionary<string, string> SortFields { get; set; }

        public List<string> CountryCollection { get; set; }
        public List<string> MediaLengthCollection { get; set; }
        public List<string> ImageQualityCollection { get; set; }

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

        public override Task LoadMediasAsync()
        {
            this.SearchQueryModel.Items.Add(new ConditionItem("MediaType", QueryMethod.LessThanOrEqual, (int)PubilcEnum.MediaType.Movie));
            if (!string.IsNullOrEmpty(SelectCountry) && SelectCountry != "不限")
            {
                this.SearchQueryModel.Items.Add(new ConditionItem("Countries", QueryMethod.Equal, SelectCountry));
            }
            return base.LoadMediasAsync();
        }

        private void RegisterCommands()
        {
            PlayMediaCommand = new RelayCommand<Media>(async (media) =>
            {
                await DialogHost.Show(new SampleLoading(), "RootDialog", (object sender, DialogOpenedEventArgs eventArgs) =>
                {
                    Task.Factory.StartNew(() => Thread.Sleep(1000)).ContinueWith(async (t) =>
                    {
                        var movie = await SimpleIoc.Default.GetInstance<IMediaService>().GetEntityAsync(e => e.ID == media.ID);
                        Messenger.Default.Send(new LoadMediaMessage(movie));
                        eventArgs.Session.Close(false);
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }, (object sender, DialogClosingEventArgs eventArgs) => { });
            });
        }
    }
}
