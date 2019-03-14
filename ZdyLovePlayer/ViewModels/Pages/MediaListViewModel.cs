using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Infrastructure.SearchModel.Model;
using NLog;
using ZdyLovePlayer.Messaging;
using ZdyLovePlayer.Models;
using ZdyLovePlayer.Services;
using GalaSoft.MvvmLight.Ioc;
using ZdyLovePlayer.Untils;
using ZdyLovePlayer.Untils.Extension;
using ZdyLovePlayer.ViewModels.Reused;
using ZdyLovePlayer.UserControls.Dialogs;

namespace ZdyLovePlayer.ViewModels.Pages
{
    public class MediaListViewModel : BasePaginationViewModel<Media>
    {
        #region Constructor

        public IMediaService _mediaService { get; set; }

        public MediaListViewModel(IApplicationState applicationState, IMediaService mediaService)
            : base(applicationState)
        {
            _mediaService = mediaService;
        }

        #endregion

        #region Properties

        private Media _currentMedia = new Media();
        public Media CurrentMedia
        {
            get { return _currentMedia; }
            set { Set(() => CurrentMedia, ref _currentMedia, value); }
        }

        #endregion

        #region Methods

        public override void ExecuteLoadMedias(bool isRefresh = false)
        {
            MediaLoadBefore?.Invoke(this, new EventArgs());

            ExecuteLoadMedias(() => _mediaService.GetMediasForPagingAsync(PageIndex, PageSize, OrderField, false, SearchQueryModel).Result, isRefresh);

            MediaLoadAfter?.Invoke(this, new EventArgs());
        }

        private void ChangeToDataBase()
        {
            //Task.Run(async () =>
            //{
            //    var _mediaService = SimpleIoc.Default.GetInstance<IMediaService>();
            //    var media = EmitMapper.ObjectMapperManager.DefaultInstance.GetMapper<Media, Media>().Map(this);
            //    await _mediaService.UpdateAsync(media);
            //}).ConfigureAwait(false);
        }

        #endregion

        #region Events

        public event EventHandler<EventArgs> MediaLoadBefore;
        public event EventHandler<EventArgs> MediaLoadAfter;

        #endregion

        #region Commands

        public RelayCommand SearchMediaCommand => new Lazy<RelayCommand>(() => new RelayCommand(() =>
        {
            SearchQueryModel.Items.Clear();
            if (!string.IsNullOrEmpty(this.SearchQuery))
            {
                SearchQueryModel.Items.Add(new ConditionItem("Code", QueryMethod.Contains, this.SearchQuery, "media"));
                SearchQueryModel.Items.Add(new ConditionItem("Title", QueryMethod.Contains, this.SearchQuery, "media"));
                SearchQueryModel.Items.Add(new ConditionItem("Actors", QueryMethod.Contains, this.SearchQuery, "media"));
                SearchQueryModel.Items.Add(new ConditionItem("Keyword", QueryMethod.Equal, this.SearchQuery, "media"));
            }

            ExecuteLoadMedias(true);
        })).Value;

        public RelayCommand<IMedia> PlayMediaCommand => new Lazy<RelayCommand<IMedia>>(() => new RelayCommand<IMedia>((media) =>
        {
            Messenger.Default.Send(new OpenMediaMessage(media));
        })).Value;

        public RelayCommand<IMedia> ShowMediaInfoCommand => new Lazy<RelayCommand<IMedia>>(() => new RelayCommand<IMedia>((media) =>
        {
            MediaInfoDialog dialog = new MediaInfoDialog
            {
                DataContext = media
            };
            _applicationState.ShowDialog(dialog);
        })).Value;

        #endregion
    }
}
