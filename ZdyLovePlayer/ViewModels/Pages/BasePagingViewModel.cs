using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Infrastructure.SearchModel.Model;
using NLog;
using ZdyLovePlayer.Messaging;
using ZdyLovePlayer.Models;
using ZdyLovePlayer.Services;
using ZdyLovePlayer.Untils;

namespace ZdyLovePlayer.ViewModels.Pages
{
    public abstract class BasePagingViewModel : ViewModelBase
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected QueryModel SearchQueryModel = new QueryModel();

        #region Constructor

        public IApplicationState _applicationState { get; set; }

        public BasePagingViewModel(IApplicationState applicationState)
        {
            _applicationState = applicationState;
        }

        #endregion

        #region Properties

        private int _pageSize = Constants.LoadDataPageSize;

        public int PageSize
        {
            get => _pageSize;
            set => Set(ref _pageSize, value);
        }

        private int _pageIndex;
        protected int PageIndex
        {
            get { return _pageIndex; }
            set { Set(() => PageIndex, ref _pageIndex, value); }
        }

        private int _currentNumberOfData;
        public int CurrentNumberOfData
        {
            get { return _currentNumberOfData; }
            set { Set(() => CurrentNumberOfData, ref _currentNumberOfData, value); }
        }

        private int _maxNumberOfData;
        public int MaxNumberOfData
        {
            get { return _maxNumberOfData; }
            set { Set(() => MaxNumberOfData, ref _maxNumberOfData, value); }
        }

        private string _orderField;
        public string OrderField
        {
            get { return _orderField; }
            set { Set(() => OrderField, ref _orderField, value); }
        }

        private bool _isLoadingFailed;
        public bool IsLoadingFailed
        {
            get { return _isLoadingFailed; }
            set { Set(() => IsLoadingFailed, ref _isLoadingFailed, value); }
        }

        private bool _isDataLoading;
        public bool IsDataLoading
        {
            get { return _isDataLoading; }
            protected set { Set(() => IsDataLoading, ref _isDataLoading, value); }
        }

        private int _loadingProgress = 0;

        public int LoadingProgress
        {
            get => _loadingProgress;
            set => Set(ref _loadingProgress, value);
        }

        private bool _isDataFound = true;
        public bool IsDataFound
        {
            get { return _isDataFound; }
            set { Set(() => IsDataFound, ref _isDataFound, value); }
        }

        #endregion

        #region Methods

        public abstract void LoadMedias(bool isRefresh = false);

        public async Task<Tuple<List<T>, int>> ExecuteDataDelay<T>(Func<Tuple<List<T>, int>> action, int delay = 1000)
        {
            var watcher = new Stopwatch();

            watcher.Start();

            var results = action.Invoke();

            watcher.Stop();

            var elapsedMilliseconds = watcher.ElapsedMilliseconds;
            if (elapsedMilliseconds < delay)
            {
                await Task.Delay(delay - (int)elapsedMilliseconds).ConfigureAwait(false);
            }

            return results;
        }

        public CancellationTokenSource CancelTokenSource { get; set; }

        public void ExecuteActionMaybeCanceled(Action action, CancellationToken cancelToken)
        {
            if (cancelToken.CanBeCanceled) cancelToken.ThrowIfCancellationRequested();

            if (!cancelToken.IsCancellationRequested) action.Invoke();
        }

        public void ExecuteActionWithProgress(Action action)
        {
            if (CancelTokenSource != null)
            {
                CancelTokenSource.Cancel();
                IsDataLoading = false;
                LoadingProgress = 0;
            }

            CancelTokenSource = new CancellationTokenSource();
            CancellationToken cancelToken = CancelTokenSource.Token;

            Task.Factory.StartNew(async () =>
            {
                ExecuteActionMaybeCanceled(() => IsDataLoading = true, cancelToken);

                for (var i = LoadingProgress; i < 80; i++)
                {
                    ExecuteActionMaybeCanceled(() => LoadingProgress = i, cancelToken);

                    await Task.Delay(10);
                }

                ExecuteActionMaybeCanceled(action, cancelToken);

                for (var i = LoadingProgress; i < 100; i++)
                {
                    ExecuteActionMaybeCanceled(() => LoadingProgress = i, cancelToken);

                    await Task.Delay(20);
                }

                ExecuteActionMaybeCanceled(() => IsDataLoading = false, cancelToken);

            }, cancelToken);
        }

        public void ExecuteActionAndWatchPageTurning(Action action)
        {
            var watcher = Stopwatch.StartNew();

            try
            {
                PageIndex++;
                IsLoadingFailed = false;
                Logger.Info($"Loading page {PageIndex}...");

                action.Invoke();
            }
            catch (Exception exception)
            {
                Messenger.Default.Send(new ManageExceptionMessage(exception));

                PageIndex--;
                IsLoadingFailed = true;
                Logger.Error($"Error while loading page {PageIndex}: {exception.Message}");
            }
            finally
            {
                watcher.Stop();
                Logger.Info($"Loaded page {PageIndex} in {watcher.ElapsedMilliseconds} milliseconds.");
            }
        }


        #endregion

        #region Commands

        public RelayCommand RefreshMediaListCommand => new Lazy<RelayCommand>(() => new RelayCommand(() => LoadMedias(true))).Value;

        #endregion
    }
}
