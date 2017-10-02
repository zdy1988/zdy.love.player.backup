using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using NLog;
using Zhu.Controls;
using Zhu.Messaging;
using Zhu.Models;
using Zhu.UserControls;
using System;
using System.Threading;
using System.Threading.Tasks;
using Zhu.UserControls.Reused;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Ioc;
using Zhu.Services;

namespace Zhu.ViewModels.Player
{
    public class MediaPlayerViewModel: ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public IMedia Media;

        public IApplicationState _applicationState { get; set; }

        public MediaPlayerViewModel(IApplicationState applicationState)
        {
            _applicationState = applicationState;

            RegisterCommands();
            RegisterMessages();
        }

        public event EventHandler<EventArgs> StartPlayingMedia;

        public event EventHandler<EventArgs> StoppedPlayingMedia;

        public override void Cleanup()
        {
            StoppedPlayingMedia?.Invoke(this, new EventArgs());
            base.Cleanup();
        }

        public async Task HasSeenMovie()
        {
            try
            {
                Logger.Info($"Seen Media {Media.Title}...");

                await SimpleIoc.Default.GetInstance<ISeenService>().InsertAsync(new Seen
                {
                    MediaSource = Media.MediaSource,
                    SeenDate = DateTime.Now,
                    Title = Media.Title,
                    MediaID = Media.ID
                });
            }
            catch
            {
                Logger.Error($"Error Seen Media {Media.Title}...");
            }
        }

        public RelayCommand StopPlayingMediaCommand { get; private set; }

        private void RegisterCommands()
        {
            StopPlayingMediaCommand = new RelayCommand(async () =>
            {
                _applicationState.ShowLoadingDialog();

                await Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(t =>
                {
                    _applicationState.HideLoadingDialog();

                    StoppedPlayingMedia?.Invoke(this, new EventArgs());
                    Messenger.Default.Send(new MediaFlyoutCloseMessage());
                }, TaskScheduler.FromCurrentSynchronizationContext());
            });
        }

        private void RegisterMessages()
        {
            Messenger.Default.Register<LoadMediaMessage>(this, async (e) =>
             {
                 _applicationState.ShowLoadingDialog();

                 await Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith((t) =>
                 {
                     _applicationState.HideLoadingDialog();

                     //检测媒体
                     if (e.Media == null || string.IsNullOrEmpty(e.Media.MediaSource))
                     {
                         Messenger.Default.Send(new MediaFlyoutCloseMessage());
                         Messenger.Default.Send(new ManageExceptionMessage(new Exception("未找到网络地址或媒体文件！")));
                         return;
                     }
                     Media = e.Media;
                     //触发播放
                     StartPlayingMedia?.Invoke(this, new EventArgs());
                 }, TaskScheduler.FromCurrentSynchronizationContext());
             });
        }
    }
}
