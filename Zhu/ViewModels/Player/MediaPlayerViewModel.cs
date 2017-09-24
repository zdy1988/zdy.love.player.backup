using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using NLog;
using Zhu.Controls;
using Zhu.Messaging;
using Zhu.Models;
using Zhu.Models.ApplicationState;
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

        public Media Media;

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
                await DialogHost.Show(new SampleLoading(), "RootDialog", (object sender, DialogOpenedEventArgs eventArgs) =>
                {
                    Task.Factory.StartNew(() => Thread.Sleep(1000)).ContinueWith(t =>
                    {
                        StoppedPlayingMedia?.Invoke(this, new EventArgs());
                        eventArgs.Session.Close(false);
                        Messenger.Default.Send(new MediaFlyoutCloseMessage());
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }, (object sender, DialogClosingEventArgs eventArgs) => { });
            });
        }

        private void RegisterMessages()
        {
            Messenger.Default.Register<LoadMediaMessage>(this, e =>
            {
                if (e.Media == null) return;
                if (string.IsNullOrEmpty(e.Media.MediaSource))
                {
                    Messenger.Default.Send(new MediaFlyoutCloseMessage());
                    Messenger.Default.Send(new ManageExceptionMessage(new Exception("未获取媒体！")));
                    return;
                }

                Media = e.Media;
                StartPlayingMedia?.Invoke(this, new EventArgs());
                Messenger.Default.Send(new MediaFlyoutOpenMessage());
            });
        }
    }
}
