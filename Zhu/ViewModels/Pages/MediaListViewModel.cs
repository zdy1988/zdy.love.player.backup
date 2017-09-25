using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Infrastructure.SearchModel.Model;
using NLog;
using Zhu.Untils;
using Zhu.Messaging;
using Zhu.Models;
using Zhu.Models.ApplicationState;
using Zhu.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Zhu.Untils.Extension;
using GalaSoft.MvvmLight.Threading;


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

        private AsyncObservableCollection<Tuple<Media, int>> _medias = new AsyncObservableCollection<Tuple<Media, int>>();
        public AsyncObservableCollection<Tuple<Media, int>> Medias
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
                    var mediaItems = new List<Tuple<Media, int>>();
                    for (var i = 0; i < medias.Item1.Count(); i++)
                    {
                        mediaItems.Add(new Tuple<Media, int>(medias.Item1[i], i + 1));
                    }

                    loadDataWatcher.Stop();
                    var elapsedMs = loadDataWatcher.ElapsedMilliseconds;
                    if (elapsedMs < 500)
                    {
                        await Task.Delay(500 - (int)elapsedMs).ConfigureAwait(false);
                    }

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        Medias.AddRange(mediaItems);
                        IsDataLoading = false;
                        IsDataFound = Medias.Any();
                        TotalNumberOfData = Medias.Count;
                        MaxNumberOfData = medias.Item2;
                    });
                });
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

        public RelayCommand<Media> PlayMediaCommand { get; set; }
        public RelayCommand<Media> SetMediaRatingCommand { get; private set; }
        public RelayCommand<Media> SetMediaFavoriteCommand { get; private set; }
        public RelayCommand SearchMediaCommand { get; private set; }

        private void RegisterCommands()
        {
            SetMediaRatingCommand = new RelayCommand<Media>(async (media) =>
            {
                var entity = _mediaService.GetEntity(t => t.ID == media.ID);
                if (entity != null)
                {
                    entity.Rating = media.Rating;
                    await _mediaService.UpdateAsync(entity);
                }
            });

            SetMediaFavoriteCommand = new RelayCommand<Media>(async (media) =>
            {
                var entity = _mediaService.GetEntity(t => t.ID == media.ID);
                if (entity != null)
                {
                    entity.IsFavorite = media.IsFavorite;
                    await _mediaService.UpdateAsync(entity);
                }
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
        }

        #endregion
    }
}
