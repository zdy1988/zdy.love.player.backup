using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Infrastructure.SearchModel.Model;
using NLog;
using Zhu.Untils;
using Zhu.Messaging;
using Zhu.Models;
using Zhu.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Zhu.Untils.Extension;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Ioc;
using Zhu.ViewModels.Reused;
using Zhu.UserControls.Home.Dialogs;

namespace Zhu.ViewModels.Pages
{
    public class MediaListViewModel : BasePagingViewModel
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public IApplicationState _applicationState { get; set; }

        public IMediaService _mediaService { get; set; }

        public MediaListViewModel(IApplicationState applicationState, IMediaService mediaService)
        {
            _applicationState = applicationState;
            _mediaService = mediaService;

            RegisterCommands();
        }

        private ObservableCollection<Tuple<MediaListItemViewModel, ListItemIsSelectViewModel>> _medias = new ObservableCollection<Tuple<MediaListItemViewModel, ListItemIsSelectViewModel>>();
        public ObservableCollection<Tuple<MediaListItemViewModel, ListItemIsSelectViewModel>> Medias
        {
            get { return _medias; }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { Set(() => SearchText, ref _searchText, value); }
        }

        public virtual async Task LoadMediasAsync(bool isRefresh = false)
        {
            if (isRefresh)
            {
                PageIndex = 0;
                Medias.Clear();
            }

            MediaBeforeLoad?.Invoke(this, new EventArgs());

            var watch = Stopwatch.StartNew();

            PageIndex++;
            HasLoadingFailed = false;
            Logger.Info($"Loading page {PageIndex}...");

            try
            {
                IsDataLoading = true;
                await Task.Run(async () =>
                {
                    var loadDataWatcher = new Stopwatch();

                    loadDataWatcher.Start();
                    var medias = await _mediaService.GetMediasForPagingAsync(PageIndex, PageSize, OrderField, false, SearchQueryModel);
                    loadDataWatcher.Stop();

                    var elapsedMs = loadDataWatcher.ElapsedMilliseconds;
                    if (elapsedMs < 500)
                    {
                        await Task.Delay(500 - (int)elapsedMs).ConfigureAwait(false);
                    }

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        for (var i = 0; i < medias.Item1.Count(); i++)
                        {
                            var item = EmitMapper.ObjectMapperManager.DefaultInstance.GetMapper<Media, MediaListItemViewModel>().Map(medias.Item1[i]);
                            var isSelect = new ListItemIsSelectViewModel { IsSelected = false };

                            Medias.Add(new Tuple<MediaListItemViewModel, ListItemIsSelectViewModel>(item, isSelect));
                        }
                        IsDataLoading = false;
                        IsDataFound = Medias.Any();
                        CurrentNumberOfData = Medias.Count;
                        MaxNumberOfData = medias.Item2;
                    });
                }).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                PageIndex--;
                HasLoadingFailed = true;
                Logger.Error($"Error while loading page {PageIndex}: {exception.Message}");
                Messenger.Default.Send(new ManageExceptionMessage(exception));
            }
            finally
            {
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Logger.Info($"Loaded page {PageIndex} in {elapsedMs} milliseconds.");
            }

            MediaLoaded?.Invoke(this, new EventArgs());
        }

        public event EventHandler<EventArgs> MediaBeforeLoad;
        public event EventHandler<EventArgs> MediaLoaded;

        #region Command

        public RelayCommand<IMedia> PlayMediaCommand { get; set; }
        public RelayCommand SearchMediaCommand { get; set; }
        public RelayCommand<IMedia> ShowMediaInfoCommand { get; set; }
        public RelayCommand RefreshMediaListCommand { get; set; }

        private void RegisterCommands()
        {
            PlayMediaCommand = new RelayCommand<IMedia>((media) =>
            {
                Messenger.Default.Send(new OpenMediaMessage(media));
            });

            SearchMediaCommand = new RelayCommand(async () =>
            {
                SearchQueryModel.Items.Clear();
                if (!string.IsNullOrEmpty(this.SearchText))
                {
                    SearchQueryModel.Items.Add(new ConditionItem("Code", QueryMethod.Contains, this.SearchText, "media"));
                    SearchQueryModel.Items.Add(new ConditionItem("Title", QueryMethod.Contains, this.SearchText, "media"));
                    SearchQueryModel.Items.Add(new ConditionItem("Actors", QueryMethod.Contains, this.SearchText, "media"));
                    SearchQueryModel.Items.Add(new ConditionItem("Keyword", QueryMethod.Equal, this.SearchText, "media"));
                }
                Medias.Clear();
                PageIndex = 0;
                await LoadMediasAsync().ConfigureAwait(false);
            });

            ShowMediaInfoCommand = new RelayCommand<IMedia>((media) =>
            {
                MediaInfoDialog dialog = new MediaInfoDialog
                {
                    DataContext = media
                };
                _applicationState.ShowDialog(dialog);
            });

            RefreshMediaListCommand = new RelayCommand(async () =>
            {
                await LoadMediasAsync(true).ConfigureAwait(false);
            });
        }

        #endregion
    }
}
