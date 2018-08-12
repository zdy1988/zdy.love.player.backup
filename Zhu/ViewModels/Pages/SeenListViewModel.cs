using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhu.Messaging;
using Zhu.Models;
using Zhu.Untils.Extension;
using Zhu.Services;
using GalaSoft.MvvmLight.Command;

namespace Zhu.ViewModels.Pages
{
    public class SeenListViewModel : BasePagingViewModel
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public IApplicationState _applicationState { get; set; }

        public ISeenService _seenService { get; set; }

        public SeenListViewModel(IApplicationState applicationState,
            ISeenService seenService)
        {
            this._applicationState = applicationState;
            this._seenService = seenService;

            this.PageSize = 100;

            RegisterCommands();
        }

        private ObservableCollection<Tuple<SeenListItemViewModel, int>> _seenMedias = new ObservableCollection<Tuple<SeenListItemViewModel, int>>();
        public ObservableCollection<Tuple<SeenListItemViewModel, int>> SeenMedias
        {
            get { return _seenMedias; }
        }

        public override async Task LoadMediasAsync(bool isRefresh = false)
        {
            if (isRefresh)
            {
                PageIndex = 0;
                SeenMedias.Clear();
            }

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
                    var data = await _seenService.GetEntitiesForPagingAsync(PageIndex, PageSize, OrderField, false, SearchQueryModel);
                    loadDataWatcher.Stop();

                    var elapsedMs = loadDataWatcher.ElapsedMilliseconds;
                    if (elapsedMs < 500)
                    {
                        await Task.Delay(500 - (int)elapsedMs).ConfigureAwait(false);
                    }

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        for (var i = 0; i < data.Item1.Count(); i++)
                        {
                            var item = new SeenListItemViewModel
                            {
                                IsSelected = false,
                                ID = data.Item1[i].ID,
                                SeenDate = data.Item1[i].SeenDate,
                                Title = data.Item1[i].Title,
                                MediaSource = data.Item1[i].MediaSource,
                                MediaID = data.Item1[i].MediaID
                            };
                            SeenMedias.Add(new Tuple<SeenListItemViewModel, int>(item, i + 1));
                        }
                        IsDataLoading = false;
                        IsDataFound = SeenMedias.Any();
                        CurrentNumberOfData = SeenMedias.Count;
                        MaxNumberOfData = data.Item2;
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
        }

        private bool? _isSelectedAll;

        public bool? IsSelectedAll
        {
            get { return _isSelectedAll; }
            set
            {
                Set(() => IsSelectedAll, ref _isSelectedAll, value);

                if (_isSelectedAll.HasValue)
                {
                    SelectAll(_isSelectedAll.Value, _seenMedias);
                }
            }
        }

        private static void SelectAll(bool select, IEnumerable<Tuple<SeenListItemViewModel, int>> models)
        {
            foreach (var model in models)
            {
                model.Item1.IsSelected = select;
            }
        }

        public RelayCommand<SeenListItemViewModel> RepalySeenMediaCommand { get; private set; }

        private void RegisterCommands()
        {
            RepalySeenMediaCommand = new RelayCommand<SeenListItemViewModel>((seen) =>
            {
                Messenger.Default.Send(new OpenMediaMessage(new Media
                {
                    ID = seen.MediaID,
                    Title = seen.Title,
                    MediaSource = seen.MediaSource
                }));
            });
        }
    }
}
