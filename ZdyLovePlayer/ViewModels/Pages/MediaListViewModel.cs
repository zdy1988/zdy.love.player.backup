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
    public class MediaListViewModel : BasePagingViewModel
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

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { Set(() => SearchText, ref _searchText, value); }
        }

        private ObservableCollection<Tuple<MediaListItemViewModel, ListItemIsSelectViewModel>> _medias = new ObservableCollection<Tuple<MediaListItemViewModel, ListItemIsSelectViewModel>>();
        public ObservableCollection<Tuple<MediaListItemViewModel, ListItemIsSelectViewModel>> Medias
        {
            get { return _medias; }
            set { Set(() => Medias, ref _medias, value); }
        }

        #endregion

        #region Methods

        public override void LoadMedias(bool isRefresh = false)
        {
            MediaLoadBefore?.Invoke(this, new EventArgs());

            if (isRefresh)
            {
                PageIndex = 0;
                Medias.Clear();
            }

            ExecuteActionAndWatchPageTurning(() => ExecuteActionWithProgress(async () =>
            {
                var medias = await ExecuteDataDelay<Media>(() => _mediaService.GetMediasForPagingAsync(PageIndex, PageSize, OrderField, false, SearchQueryModel).Result);

                var list = new ObservableCollection<Tuple<MediaListItemViewModel, ListItemIsSelectViewModel>>();
                for (var i = 0; i < medias.Item1.Count(); i++)
                {
                    var item = EmitMapper.ObjectMapperManager.DefaultInstance.GetMapper<Media, MediaListItemViewModel>().Map(medias.Item1[i]);
                    var isSelect = new ListItemIsSelectViewModel { IsSelected = false };

                    list.Add(new Tuple<MediaListItemViewModel, ListItemIsSelectViewModel>(item, isSelect));
                }

                Medias = list;
                IsDataFound = Medias.Any();
                CurrentNumberOfData = Medias.Count;
                MaxNumberOfData = medias.Item2;
            }));
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
            if (!string.IsNullOrEmpty(this.SearchText))
            {
                SearchQueryModel.Items.Add(new ConditionItem("Code", QueryMethod.Contains, this.SearchText, "media"));
                SearchQueryModel.Items.Add(new ConditionItem("Title", QueryMethod.Contains, this.SearchText, "media"));
                SearchQueryModel.Items.Add(new ConditionItem("Actors", QueryMethod.Contains, this.SearchText, "media"));
                SearchQueryModel.Items.Add(new ConditionItem("Keyword", QueryMethod.Equal, this.SearchText, "media"));
            }

            LoadMedias(true);
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
