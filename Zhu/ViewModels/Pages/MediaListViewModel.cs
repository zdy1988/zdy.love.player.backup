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
    public class MediaListViewModel<T> : ViewModelBase where T : class, IMedia
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public IApplicationState ApplicationState { get; set; }

        public IMediaService<T> MediaService { get; set; }

        public MediaListViewModel(IApplicationState applicationState, IMediaService<T> mediaService)
        {
            ApplicationState = applicationState;
            MediaService = mediaService;

            RegisterCommands();
        }

        protected int PageIndex { get; set; }

        protected int PageSize { get; set; } = Constants.LoadMoviesPageSize;

        private bool _hasLoadingFailed;
        public bool HasLoadingFailed
        {
            get { return _hasLoadingFailed; }
            set { Set(() => HasLoadingFailed, ref _hasLoadingFailed, value); }
        }

        private bool _isLoadingMedias;
        public bool IsLoadingMedias
        {
            get { return _isLoadingMedias; }
            protected set { Set(() => IsLoadingMedias, ref _isLoadingMedias, value); }
        }

        private bool _isMoviesFound = true;
        public bool IsMediasFound
        {
            get { return _isMoviesFound; }
            set { Set(() => IsMediasFound, ref _isMoviesFound, value); }
        }

        private int _currentNumberOfMedias;
        public int CurrentNumberOfMedias
        {
            get { return _currentNumberOfMedias; }
            set { Set(() => CurrentNumberOfMedias, ref _currentNumberOfMedias, value); }
        }

        private int _maxNumberOfMedias;
        public int MaxNumberOfMedias
        {
            get { return _maxNumberOfMedias; }
            set { Set(() => MaxNumberOfMedias, ref _maxNumberOfMedias, value); }
        }

        private ObservableCollection<Tuple<MediaDTO, int>> _medias = new ObservableCollection<Tuple<MediaDTO, int>>();
        public ObservableCollection<Tuple<MediaDTO, int>> Medias
        {
            get { return _medias; }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { Set(() => SearchText, ref _searchText, value); }
        }

        private string _searchOrderField;
        public string SearchOrderField
        {
            get { return _searchOrderField; }
            set { Set(() => SearchOrderField, ref _searchOrderField, value); }
        }

        protected QueryModel SearchQueryModel = new QueryModel();

        protected virtual async Task LoadMediasAsync()
        {
            var watch = Stopwatch.StartNew();

            PageIndex++;

            Logger.Info($"Loading page {PageIndex}...");

            HasLoadingFailed = false;

            try
            {
                IsLoadingMedias = true;
                await Task.Run(async () =>
                {
                    var getMediasWatcher = new Stopwatch();
                    getMediasWatcher.Start();

                    var medias = await MediaService.GetMediasForPagingAsync(PageIndex, PageSize, SearchOrderField, false, SearchQueryModel);
                    var mediaItems = new List<Tuple<MediaDTO, int>>();
                    for (var i = 0; i < medias.Item1.Count(); i++)
                    {
                        mediaItems.Add(new Tuple<MediaDTO, int>(medias.Item1[i], i + 1));
                    }

                    getMediasWatcher.Stop();
                    var getMediasEllapsedTime = getMediasWatcher.ElapsedMilliseconds;
                    if (getMediasEllapsedTime < 500)
                    {
                        await Task.Delay(500 - (int)getMediasEllapsedTime).ConfigureAwait(false);
                    }

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        Medias.AddRange(mediaItems);
                        IsLoadingMedias = false;
                        IsMediasFound = Medias.Any();
                        CurrentNumberOfMedias = Medias.Count;
                        MaxNumberOfMedias = medias.Item2;
                    });
                });
            }
            catch (Exception exception)
            {
                PageIndex--;
                Logger.Error($"Error while loading page {PageIndex}: {exception.Message}");
                HasLoadingFailed = true;
                Messenger.Default.Send(new ManageExceptionMessage(exception));
            }
            finally
            {
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Logger.Info($"Loaded page {PageIndex} in {elapsedMs} milliseconds.");
            }
        }

        private ObservableCollection<Tuple<string, string>> _tags = new ObservableCollection<Tuple<string, string>>();

        public ObservableCollection<Tuple<string, string>> Tags
        {
            get { return _tags; }
        }

        protected virtual async Task LoadMeidaTags(string type)
        {
            await Task.Run(async () =>
            {
                var queryModel = new QueryModel();
                queryModel.Items.Add(new ConditionItem("Type", QueryMethod.Equal, type));
                var tags = await GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<ITagService>().GetTagsForPagingAsync(1, 50, queryModel);
                //if (tags.Item1.Count > 0)
                //{
                //    for (var i = 0; i < tags.Item1.Count; i++)
                //    {
                //        Dispatcher.CurrentDispatcher.Invoke(() =>
                //        {
                //            _tags.Add(new Tuple<string, string>(tags.Item1[i], tags.Item1[i].Substring(0, 1)));
                //        }, DispatcherPriority.Background);
                //    }
                //}
            });
        }

        public RelayCommand<Media> PlayMediaCommand { get; set; }
        public RelayCommand<Media> SetMediaRatingCommand { get; private set; }
        public RelayCommand<Media> SetMediaFavoriteCommand { get; private set; }
        public RelayCommand SortMediaCommand { get; private set; }
        public RelayCommand LoadMediaCommand { get; private set; }
        public RelayCommand SearchMediaCommand { get; private set; }
        public RelayCommand<string> LoadMeidaTagCommand { get; private set; }
        public RelayCommand<string> DeleteTagCommand { get; private set; }

        public event EventHandler<EventArgs> MediaBeforeLoad;
        public event EventHandler<EventArgs> MediaLoaded;

        private void RegisterCommands()
        {
            SetMediaRatingCommand = new RelayCommand<Media>(async (media) =>
            {
                var entity = MediaService.GetEntity(t => t.ID == media.ID);
                if (entity != null)
                {
                    entity.Rating = media.Rating;
                    await MediaService.UpdateAsync(entity);
                }
            });

            SetMediaFavoriteCommand = new RelayCommand<Media>(async (media) =>
            {
                var entity = MediaService.GetEntity(t => t.ID == media.ID);
                if (entity != null)
                {
                    entity.IsFavorite = media.IsFavorite;
                    await MediaService.UpdateAsync(entity);
                }
            });

            LoadMediaCommand = new RelayCommand(async () =>
            {
                MediaBeforeLoad?.Invoke(this, new EventArgs());
                await LoadMediasAsync().ConfigureAwait(false);
                MediaLoaded?.Invoke(this, new EventArgs());
            });

            SortMediaCommand = new RelayCommand(() =>
            {
                Medias.Clear();
                PageIndex = 0;
                this.LoadMediaCommand.Execute(null);
            });

            SearchMediaCommand = new RelayCommand(() =>
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
                this.LoadMediaCommand.Execute(null);
            });

            LoadMeidaTagCommand = new RelayCommand<string>(async (type) => {
                await LoadMeidaTags(type);
            });

            DeleteTagCommand = new RelayCommand<string>((keyWord) =>
            {
                _tags.Where(t => t.Item1 == keyWord).ToList().ForEach(t =>
                {
                    _tags.Remove(t);
                });
            });
        }
    }
}
