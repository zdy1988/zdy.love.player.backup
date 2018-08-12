using System;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using NLog;
using Unosquare.FFME;
using Unosquare.FFME.Platform;
using Zhu.Services;
using Zhu.Foundation;
using Zhu.Messaging;
using Zhu.Models;

namespace Zhu.ViewModels.Player
{
    public class MediaPlayerViewModel: ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public IMedia Media;

        public MediaPlayerControllerViewModel Controller { get; }

        public MediaPlayerPlaylistViewModel Playlist { get; }

        public IApplicationState _applicationState { get; set; }

        private string _windowTitle = string.Empty;

        public string WindowTitle
        {
            get { return _windowTitle; }
            set { Set(() => WindowTitle, ref _windowTitle, value); }
        }

        private MediaElement _mediaElement;

        public MediaElement MediaElement
        {
            get { return _mediaElement; }
            set { Set(() => MediaElement, ref _mediaElement, value); }
        }

        private bool _isMediaPlayerLoaded = GuiContext.Current.IsInDesignTime;

        public bool IsMediaPlayerLoaded
        {
            get { return _isMediaPlayerLoaded; }
            set { Set(() => IsMediaPlayerLoaded, ref _isMediaPlayerLoaded, value); }
        }

        private bool _isPropertiesPanelOpen = GuiContext.Current.IsInDesignTime;

        public bool IsPropertiesPanelOpen
        {
            get { return _isPropertiesPanelOpen; }
            set { Set(() => IsPropertiesPanelOpen, ref _isPropertiesPanelOpen, value); }
        }

        private bool _isPlaylistPanelOpen = GuiContext.Current.IsInDesignTime;

        public bool IsPlaylistPanelOpen
        {
            get { return _isPlaylistPanelOpen; }
            set { Set(() => IsPlaylistPanelOpen, ref _isPlaylistPanelOpen, value); }
        }

        public MediaPlayerViewModel(IApplicationState applicationState)
        {
            _applicationState = applicationState;

            Controller = new MediaPlayerControllerViewModel(this);
            Playlist = new MediaPlayerPlaylistViewModel(this);

            RegisterCommands();
            RegisterMessages();
        }

        public RelayCommand<string> OpenMediaCommand { get; private set; }
        public RelayCommand PlayMediaCommand { get; private set; }
        public RelayCommand CloseMediaCommand { get; private set; }
        public RelayCommand PauseMediaCommand { get; private set; }
        public RelayCommand StopMediaCommand { get; private set; }
        public RelayCommand<CustomPlaylistEntry> RemovePlaylistItemCommand { get; private set; }

        private void RegisterCommands()
        {
            OpenMediaCommand =  new RelayCommand<string>(async (url) =>
            {
                Media = new Media
                {
                    MediaSource = url
                };
                await OpenMediaAsync();
            });

            PlayMediaCommand = new RelayCommand(async () =>
            {
                if (MediaElement.HasVideo || MediaElement.HasAudio)
                {
                    if (MediaElement.HasMediaEnded)
                    {
                        await OpenMediaAsync();
                    }
                    else
                    {
                        await MediaElement.Play();
                    }
                }
                else
                {
                    Messenger.Default.Send(new PlayMediaSourceDialogOpenMessage());
                }
            });

            CloseMediaCommand = new RelayCommand(async () =>
            {
                _applicationState.ShowLoadingDialog();
                await MediaElement.Close();
                await Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(async (t) =>
                {
                    _applicationState.HideLoadingDialog();
                    Messenger.Default.Send(new MediaFlyoutCloseMessage());
                }, TaskScheduler.FromCurrentSynchronizationContext());
            });

            PauseMediaCommand = new RelayCommand(async () => {
                if (MediaElement.CanPause)
                {
                    await MediaElement.Pause();
                }
            });

            StopMediaCommand = new RelayCommand(async () => {
                await MediaElement.Stop();
            });

            RemovePlaylistItemCommand = new RelayCommand<CustomPlaylistEntry>((entry) =>
            {
                Playlist.Entries.RemoveEntryByMediaUrl(entry.MediaUrl);
                Playlist.Entries.SaveEntries();
            });
        }

        private void RegisterMessages()
        {
            Messenger.Default.Register<OpenMediaMessage>(this, async (e) =>
            {
                _applicationState.ShowLoadingDialog();

                await Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(async (t) =>
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
                    await OpenMediaAsync();

                }, TaskScheduler.FromCurrentSynchronizationContext());
            });
        }

        public async Task OpenMediaAsync()
        {
            if (Media == null) return;
            if (Media.MediaSource == null) return;

            Messenger.Default.Send(new MediaFlyoutOpenMessage());
            try
            {
                var uriString = Media.MediaSource as string;
                if (string.IsNullOrWhiteSpace(uriString))
                    return;

                // you can also set the source to the Uri to open
                // Current.MediaElement.Source = new Uri(uriString);
                
                var target = new Uri(uriString);
                if (target.ToString().StartsWith(FileInputStream.Scheme))
                {
                    await MediaElement.Open(new FileInputStream(target.LocalPath));
                }
                else
                {
                    await MediaElement.Open(target);
                }

                this.IsPlaylistPanelOpen = false;
                this.IsPropertiesPanelOpen = false;
            }
            catch (Exception ex)
            {
                Logger.Error($"Media Failed: {ex.GetType()}\r\n{ex.Message},{nameof(MediaElement)} Error");
            }
        }

        public async Task RecordSeenMovieAsync()
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

        public void MediaPlayerLoaded()
        {
            if (IsMediaPlayerLoaded)
                return;

            Playlist.MediaPlayerLoaded();
            Controller.MediaPlayerLoaded();

            var m = MediaElement;
            new Action(UpdateWindowTitle).WhenChanged(m,
                nameof(m.IsOpen),
                nameof(m.IsOpening),
                nameof(m.MediaState),
                nameof(m.Source));

            m.MediaOpened += (s, e) =>
            {
                // Reset the Zoom
                Controller.MediaElementZoom = 1d;

                // Update the Controls
                Playlist.IsInOpenMode = false;
                IsMediaPlayerLoaded = false;
                Playlist.OpenTargetUrl = m.Source.ToString();
            };

            //IsPlaylistPanelOpen = true;
            IsMediaPlayerLoaded = true;
        }

        private void UpdateWindowTitle()
        {
            var title = MediaElement?.Source?.ToString() ?? "(No media loaded)";
            var state = MediaElement?.MediaState.ToString();

            if (MediaElement?.IsOpen ?? false)
            {
                foreach (var kvp in MediaElement.Metadata)
                {
                    if (kvp.Key.ToLowerInvariant().Equals("title"))
                    {
                        title = kvp.Value;
                        break;
                    }
                }
            }
            else if (MediaElement?.IsOpening ?? false)
            {
                state = "Opening . . .";
            }
            else
            {
                title = "(No media loaded)";
                state = "Ready";
            }

            WindowTitle = $"{title} - {state} - Unosquare FFME Play v1.0 "
                + $"FFmpeg {MediaElement.FFmpegVersionInfo} ({(GuiContext.Current.IsInDebugMode ? "Debug" : "Release")})";
        }
    }
}
