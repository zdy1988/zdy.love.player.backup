using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using NLog;
using ZDY.LovePlayer.Messaging;
using ZDY.LovePlayer.Services;

namespace ZDY.LovePlayer.ViewModels
{
    public class ViewModelBasic: ViewModelBase
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #region Constructor

        public IApplicationState _applicationState { get; set; }

        public ViewModelBasic(IApplicationState applicationState)
        {
            _applicationState = applicationState;
        }

        #endregion

        #region Properties

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

        #endregion

        #region Methods

        public async Task<Tuple<List<T>, int>> ExecuteLoadDataWithDelay<T>(Func<Tuple<List<T>, int>> action, int delay = 1000)
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

        public void ExecuteActionWithWatching(string message, params Action[] actions)
        {
            var watcher = Stopwatch.StartNew();

            try
            {
                Logger.Info($"Execute Action {message}...");

                actions[0]?.Invoke();
            }
            catch (Exception exception)
            {
                actions[1]?.Invoke();

                Messenger.Default.Send(new ManageExceptionMessage(exception));

                Logger.Error($"Execute Action {message}. Error: {exception.Message}");
            }
            finally
            {
                watcher.Stop();

                Logger.Info($"Execute Action {message} In {watcher.ElapsedMilliseconds} Milliseconds.");
            }
        }

        #endregion
    }
}
