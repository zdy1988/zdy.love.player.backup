using GalaSoft.MvvmLight.Messaging;
using Zhu.Messaging;
using Zhu.Models;
using Zhu.ViewModels.Player;
using Zhu.Untils;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Meta.Vlc.Wpf;
using GalaSoft.MvvmLight.Threading;

namespace Zhu.UserControls
{
    /// <summary>
    /// MovieZhu.xaml 的交互逻辑
    /// </summary>
    public partial class MediaPlayer : UserControl, IDisposable
    {
        public MediaPlayer()
        {
            InitializeComponent();

            Loaded += MoviePlayer_Loaded;
        }

        protected bool MediaPlayerIsPlaying { get; set; }

        private void MoviePlayer_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
                window.Closing += (s1, e1) => Dispose();

            var vm = DataContext as MediaPlayerViewModel;
            if (vm == null) return;
            vm.StartPlayingMedia += OnStartPlayingMedia;
            vm.StoppedPlayingMedia += OnStoppedPlayingMedia;

            // start the activity timer used to manage visibility of the PlayerStatusBar
            ActivityTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
            ActivityTimer.Tick += OnInactivity;
            ActivityTimer.Start();

            InputManager.Current.PreProcessInput += OnActivity;

            Player.VlcMediaPlayer.TimeChanged += VlcMediaPlayer_TimeChanged;
            Player.VlcMediaPlayer.Playing += VlcMediaPlayer_Playing;
            Player.VlcMediaPlayer.Stoped += VlcMediaPlayer_Stoped;
            Player.VlcMediaPlayer.EndReached += VlcMediaPlayer_EndReached;
            Player.VlcMediaPlayer.EncounteredError += VlcMediaPlayer_EncounteredError;
        }

        #region VlcPlayer

        private void VlcMediaPlayer_TimeChanged(object sender, EventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if ((Player == null) || (UserIsDraggingMediaPlayerSlider)) return;
                MediaPlayerSliderProgress.Value = Player.Time.TotalSeconds;
            });
        }

        private void VlcMediaPlayer_Playing(object sender, Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Media.MediaState> e)
        {
            MediaPlayerIsPlaying = true;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                MediaPlayerSliderProgress.Minimum = 0;
                MediaPlayerSliderProgress.Maximum = Player.Length.TotalSeconds;
            });
        }

        private void VlcMediaPlayer_Stoped(object sender, Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Media.MediaState> e)
        {
            MediaPlayerIsPlaying = false;
            DispatcherHelper.CheckBeginInvokeOnUI(async () =>
            {
                Messenger.Default.Send(new MediaFlyoutCloseMessage());
                var vm = DataContext as MediaPlayerViewModel;
                await vm.HasSeenMovie();
            });
        }

        private void VlcMediaPlayer_EndReached(object sender, EventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                MediaPlayerIsPlaying = false;
            });
        }

        private void VlcMediaPlayer_EncounteredError(object sender, EventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Messenger.Default.Send(new ManageExceptionMessage(new Exception("媒体错误或者媒体地址未找到！")));
                Messenger.Default.Send(new MediaFlyoutCloseMessage());
            });
        }

        #endregion

        private void OnStartPlayingMedia(object sender, EventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                var vm = DataContext as MediaPlayerViewModel;
                if (vm == null) return;
                if (vm.Media == null) return;
                if (vm.Media.MediaSource == null) return;
                //字幕
                //if (!string.IsNullOrEmpty(vm.Movie.SelectedSubtitle?.FilePath))
                //{
                //    Zhu.LoadMediaWithOptions(vm.Movie.FilePath, $@":sub-file={vm.Movie.SelectedSubtitle?.FilePath}");
                //}
                //else
                //{
                //    Zhu.LoadMedia(vm.Movie.FilePath);
                //}
                Player.Stop();
                Player.LoadMedia(vm.Media.MediaSource);
                PlayMedia();
            });
        }

        private void OnStoppedPlayingMedia(object sender, EventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (Player.VlcMediaPlayer.IsPlaying)
                {
                    Player.Stop();
                    MediaPlayerIsPlaying = false;
                }
            });
        }

        protected DispatcherTimer ActivityTimer { get; set; }

        protected Point InactiveMousePosition { get; set; } = new Point(0, 0);

        private bool _isMouseActivityCaptured;

        private void OnInactivity(object sender, EventArgs e)
        {
            InactiveMousePosition = Mouse.GetPosition(PlayerContainer);

            var opacityAnimation = new DoubleAnimationUsingKeyFrames
            {
                Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                KeyFrames = new DoubleKeyFrameCollection
                {
                    new EasingDoubleKeyFrame(0.0, KeyTime.FromPercent(1d), new PowerEase
                    {
                        EasingMode = EasingMode.EaseInOut
                    })
                }
            };

            PlayerStatusBar.BeginAnimation(OpacityProperty, opacityAnimation);
            PlayerReturnButton.BeginAnimation(OpacityProperty, opacityAnimation);
        }

        private async void OnActivity(object sender, PreProcessInputEventArgs e)
        {
            if (_isMouseActivityCaptured)
                return;

            _isMouseActivityCaptured = true;

            var inputEventArgs = e.StagingItem.Input;
            if (!(inputEventArgs is MouseEventArgs) && !(inputEventArgs is KeyboardEventArgs))
            {
                _isMouseActivityCaptured = false;
                return;
            }
            var mouseEventArgs = e.StagingItem.Input as MouseEventArgs;

            // no button is pressed and the position is still the same as the application became inactive
            if (mouseEventArgs?.LeftButton == MouseButtonState.Released &&
                mouseEventArgs.RightButton == MouseButtonState.Released &&
                mouseEventArgs.MiddleButton == MouseButtonState.Released &&
                mouseEventArgs.XButton1 == MouseButtonState.Released &&
                mouseEventArgs.XButton2 == MouseButtonState.Released &&
                InactiveMousePosition == mouseEventArgs.GetPosition(PlayerContainer))
            {
                _isMouseActivityCaptured = false;
                return;
            }

            var opacityAnimation = new DoubleAnimationUsingKeyFrames
            {
                Duration = new Duration(TimeSpan.FromSeconds(0.1)),
                KeyFrames = new DoubleKeyFrameCollection
                {
                    new EasingDoubleKeyFrame(1.0, KeyTime.FromPercent(1.0), new PowerEase
                    {
                        EasingMode = EasingMode.EaseInOut
                    })
                }
            };

            PlayerStatusBar.BeginAnimation(OpacityProperty, opacityAnimation);
            PlayerReturnButton.BeginAnimation(OpacityProperty, opacityAnimation);

            await Task.Delay(TimeSpan.FromSeconds(1));
            _isMouseActivityCaptured = false;
        }

        #region ChangeMediaVolume

        public int Volume
        {
            get { return (int)GetValue(VolumeProperty); }

            set { SetValue(VolumeProperty, value); }
        }

        internal static readonly DependencyProperty VolumeProperty = DependencyProperty.Register("Volume",
            typeof(int),
            typeof(MediaPlayer), new PropertyMetadata(100, OnVolumeChanged));

        private static void OnVolumeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var moviePlayer = obj as MediaPlayer;
            if (moviePlayer == null)
                return;

            var newVolume = (int)e.NewValue;
            moviePlayer.ChangeMediaVolume(newVolume);
        }

        private void ChangeMediaVolume(int newValue) => Player.Volume = newValue;

        private void PlayerContainer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Volume <= 190 && e.Delta > 0) || (Volume >= 10 && e.Delta < 0))
                Volume += (e.Delta > 0) ? 10 : -10;
        }

        #endregion

        #region MediaSliderProgressValueChanged

        protected bool UserIsDraggingMediaPlayerSlider { get; set; }

        private void MediaPlayerSliderProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                MoviePlayerTextProgressStatus.Text =
                  TimeSpan.FromSeconds(MediaPlayerSliderProgress.Value)
                      .ToString(@"hh\:mm\:ss", CultureInfo.CurrentCulture) + " / " +
                  TimeSpan.FromSeconds(Player.Length.TotalSeconds)
                      .ToString(@"hh\:mm\:ss", CultureInfo.CurrentCulture);
            });
        }

        protected void MediaPlayerSliderProgress_DragStarted(object sender, DragStartedEventArgs e) => UserIsDraggingMediaPlayerSlider = true;

        private void MediaPlayerSliderProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            UserIsDraggingMediaPlayerSlider = false;
            Player.Time = TimeSpan.FromSeconds(MediaPlayerSliderProgress.Value);
        }

        private void MediaPlayerSliderProgress_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Player.Time = TimeSpan.FromSeconds(MediaPlayerSliderProgress.Value);
        }

        private void MediaPlayerSliderProgress_MouseMove(object sender, MouseEventArgs e)
        {
            var rate = e.GetPosition(MediaPlayerSliderProgress).X / MediaPlayerSliderProgress.ActualWidth;
            MediaPlayerSliderProgress.ToolTip = TimeSpan.FromSeconds(MediaPlayerSliderProgress.Maximum * rate).ToString(@"hh\:mm\:ss", CultureInfo.CurrentCulture);
        }

        #endregion

        #region PlayMedia

        private void PlayMedia()
        {
            Player.Play();
            if (Player.VlcMediaPlayer.CanPlay)
            {
                MediaPlayerIsPlaying = true;

                MediaPlayerStatusBarItemPlay.Visibility = Visibility.Collapsed;
                MediaPlayerStatusBarItemPause.Visibility = Visibility.Visible;
            }
            else
            {
                Messenger.Default.Send(new SelectMediaFileDialogOpenMessage());
            }
        }

        private void MediaPlayerPlayCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (MediaPlayerStatusBarItemPlay == null || MediaPlayerStatusBarItemPause == null) return;
            e.CanExecute = Player != null;

            if (MediaPlayerIsPlaying)
            {
                MediaPlayerStatusBarItemPlay.Visibility = Visibility.Collapsed;
                MediaPlayerStatusBarItemPause.Visibility = Visibility.Visible;
            }
            else
            {
                MediaPlayerStatusBarItemPlay.Visibility = Visibility.Visible;
                MediaPlayerStatusBarItemPause.Visibility = Visibility.Collapsed;
            }
        }

        private void MediaPlayerPlayExecuted(object sender, ExecutedRoutedEventArgs e) => PlayMedia();

        #endregion

        #region PauseMedia

        private void PauseMedia()
        {
            Player.Pause();
            if (Player.VlcMediaPlayer.CanPause)
            {
                MediaPlayerIsPlaying = false;

                MediaPlayerStatusBarItemPlay.Visibility = Visibility.Visible;
                MediaPlayerStatusBarItemPause.Visibility = Visibility.Collapsed;
            }
        }

        private void MediaPlayerPauseCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (MediaPlayerStatusBarItemPlay == null || MediaPlayerStatusBarItemPause == null) return;
            e.CanExecute = MediaPlayerIsPlaying;
            if (MediaPlayerIsPlaying)
            {
                MediaPlayerStatusBarItemPlay.Visibility = Visibility.Collapsed;
                MediaPlayerStatusBarItemPause.Visibility = Visibility.Visible;
            }
            else
            {
                MediaPlayerStatusBarItemPlay.Visibility = Visibility.Visible;
                MediaPlayerStatusBarItemPause.Visibility = Visibility.Collapsed;
            }
        }

        private void MediaPlayerPauseExecuted(object sender, ExecutedRoutedEventArgs e) => PauseMedia();

        #endregion

        #region Dispose

        protected bool Disposed;

        private void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            Loaded -= MoviePlayer_Loaded;

            if (ActivityTimer != null)
            {
                ActivityTimer.Tick -= OnInactivity;
                ActivityTimer.Stop();
            }

            InputManager.Current.PreProcessInput -= OnActivity;

            MediaPlayerIsPlaying = false;

            Player.VlcMediaPlayer.TimeChanged -= VlcMediaPlayer_TimeChanged;
            Player.VlcMediaPlayer.Playing -= VlcMediaPlayer_Playing;
            Player.VlcMediaPlayer.Stoped -= VlcMediaPlayer_Stoped;
            Player.VlcMediaPlayer.EndReached -= VlcMediaPlayer_EndReached;
            Player.VlcMediaPlayer.EncounteredError -= VlcMediaPlayer_EncounteredError;
            Player.Stop();
            Player.Dispose();

            var vm = DataContext as MediaPlayerViewModel;
            if (vm != null)
                vm.StoppedPlayingMedia -= OnStoppedPlayingMedia;
                vm.StartPlayingMedia -= OnStartPlayingMedia;

            Disposed = true;

            if (disposing)
                GC.SuppressFinalize(this);
        }

        public void Dispose() => Dispose(true);

        #endregion

        #region FullScreen

        private void Player_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var vm = this.DataContext as MediaPlayerViewModel;
            if (vm != null)
            {
                vm._applicationState.IsFullScreen = !vm._applicationState.IsFullScreen;
            }
        }

        #endregion

        private void MediaPlayerSliderProgress_MouseEnter(object sender, MouseEventArgs e)
        {
            UserIsDraggingMediaPlayerSlider = true;
        }

        private void MediaPlayerSliderProgress_MouseLeave(object sender, MouseEventArgs e)
        {
            UserIsDraggingMediaPlayerSlider = false;
        }
    }
}
