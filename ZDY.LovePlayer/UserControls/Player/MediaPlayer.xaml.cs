using GalaSoft.MvvmLight.Messaging;
using ZDY.LovePlayer.Messaging;
using ZDY.LovePlayer.Models;
using ZDY.LovePlayer.ViewModels.Player;
using ZDY.LovePlayer.Untils;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;
using System.Windows.Media;
using Unosquare.FFME;
using System.Linq;
using Unosquare.FFME.Shared;
using ZDY.LovePlayer.Foundation;
using Unosquare.FFME.ClosedCaptions;

namespace ZDY.LovePlayer.UserControls
{
    public partial class MediaPlayer : UserControl, IDisposable
    {
        public MediaPlayerViewModel ViewModel => DataContext as MediaPlayerViewModel;

        public MediaPlayer()
        {
            InitializeComponent();

            InitializeMediaPlayer();
            InitializeMediaEvents();
        }

        private static readonly Key[] TogglePlayPauseKeys = new[] { Key.Play, Key.MediaPlayPause, Key.Space };
        private DateTime LastMouseMoveTime;
        private Point LastMousePosition;
        private DispatcherTimer MouseMoveTimer = null;
        private MediaType StreamCycleMediaType = MediaType.None;

        private void InitializeMediaPlayer()
        {
            ViewModel.MediaElement = MediaPlayerElement;
            ViewModel.MediaPlayerLoaded();

            Loaded += OnMediaPlayerLoaded;
            PreviewKeyDown += OnWindowKeyDown;
            MouseWheel += OnMouseWheelChange;

            InitializeMouseMoveDetectionForHiding();
        }

        private void InitializeMouseMoveDetectionForHiding()
        {
            var window = Window.GetWindow(this);

            LastMouseMoveTime = DateTime.UtcNow;

            MouseMove += (s, e) =>
            {
                var currentPosition = e.GetPosition(window);
                if (currentPosition.X != LastMousePosition.X || currentPosition.Y != LastMousePosition.Y)
                    LastMouseMoveTime = DateTime.UtcNow;

                LastMousePosition = currentPosition;
            };

            MouseLeave += (s, e) =>
            {
                LastMouseMoveTime = DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(10));
            };

            MouseMoveTimer = new DispatcherTimer(DispatcherPriority.Background)
            {
                Interval = TimeSpan.FromMilliseconds(150),
                IsEnabled = true
            };

            MouseMoveTimer.Tick += (s, e) =>
            {
                var elapsedSinceMouseMove = DateTime.UtcNow.Subtract(LastMouseMoveTime);
                if (elapsedSinceMouseMove.TotalMilliseconds >= 3000 && MediaPlayerElement.IsOpen && ControllerPanel.IsMouseOver == false
                    && PropertiesPanel.Visibility != Visibility.Visible && ControllerPanel.SoundMenuPopup.IsOpen == false)
                {
                    if (ControllerPanel.Opacity != 0d)
                    {
                        Cursor = Cursors.None;
                        var sb = FindResource("HideControlOpacity") as Storyboard;
                        Storyboard.SetTarget(sb, ControllerPanel);
                        sb.Begin();
                    }
                }
                else
                {
                    if (ControllerPanel.Opacity != 1d)
                    {
                        Cursor = Cursors.Arrow;
                        var sb = FindResource("ShowControlOpacity") as Storyboard;
                        Storyboard.SetTarget(sb, ControllerPanel);
                        sb.Begin();
                    }
                }
            };

            MouseMoveTimer.Start();
        }

        private void InitializeMediaEvents()
        {
            // Global FFmpeg message handler
            Unosquare.FFME.MediaElement.FFmpegMessageLogged += OnMediaFFmpegMessageLogged;

            // MediaElement event bindings
            MediaPlayerElement.PreviewMouseDoubleClick += OnMediaDoubleClick;
            MediaPlayerElement.MediaInitializing += OnMediaInitializing;
            MediaPlayerElement.MediaOpening += OnMediaOpening;
            MediaPlayerElement.MediaOpened += OnMediaOpened;
            //MediaPlayerElement.MediaChanging += OnMediaChanging;
            //MediaPlayerElement.AudioDeviceStopped += OnAudioDeviceStopped;
            //MediaPlayerElement.MediaChanged += OnMediaChanged;
            MediaPlayerElement.PositionChanged += OnMediaPositionChanged;
            MediaPlayerElement.MediaFailed += OnMediaFailed;
            MediaPlayerElement.MessageLogged += OnMediaMessageLogged;

            // Complex examples of MEdia Rendering Events
            BindMediaRenderingEvents();
        }

        #region Window Control and Input Event Handlers

        /// <summary>
        /// Handles the Loaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnMediaPlayerLoaded(object sender, RoutedEventArgs e)
        {
            // Remove the event handler reference
            Loaded -= OnMediaPlayerLoaded;

            var window = Window.GetWindow(this);
            if (window != null)
                window.Closing += (s1, e1) => Dispose();
        }

        /// <summary>
        /// Handles the PreviewKeyDown event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void OnWindowKeyDown(object sender, KeyEventArgs e)
        {
            // Console.WriteLine($"KEY: {e.Key}, SRC: {e.OriginalSource?.GetType().Name}");
            if (e.OriginalSource is TextBox)
                return;

            // Keep the key focus on the main window
            FocusManager.SetIsFocusScope(this, true);
            FocusManager.SetFocusedElement(this, this);

            if (e.Key == Key.G)
            {
                // Example of toggling subtitle color
                if (Subtitles.GetForeground(MediaPlayerElement) == Brushes.LightYellow)
                    Subtitles.SetForeground(MediaPlayerElement, Brushes.Yellow);
                else
                    Subtitles.SetForeground(MediaPlayerElement, Brushes.LightYellow);

                return;
            }

            // Pause
            if (TogglePlayPauseKeys.Contains(e.Key) && MediaPlayerElement.IsPlaying)
            {
                ViewModel.PauseMediaCommand.Execute(null);
                return;
            }

            // Play
            if (TogglePlayPauseKeys.Contains(e.Key) && MediaPlayerElement.IsPlaying == false)
            {
                ViewModel.PlayMediaCommand.Execute(null);
                return;
            }

            // Seek to left
            //if (e.Key == Key.Left)
            //{
            //    if (MediaPlayerElement.IsPlaying) await MediaPlayerElement.Pause();
            //    MediaPlayerElement.Position = Player.PositionPrevious;

            //    return;
            //}

            // Seek to right
            //if (e.Key == Key.Right)
            //{
            //    if (MediaPlayerElement.IsPlaying) await MediaPlayerElement.Pause();
            //    //Player.Position = Player.PositionNext;

            //    return;
            //}

            // Volume Up
            if (e.Key == Key.Add || e.Key == Key.VolumeUp)
            {
                MediaPlayerElement.Volume += 0.05;
                return;
            }

            // Volume Down
            if (e.Key == Key.Subtract || e.Key == Key.VolumeDown)
            {
                MediaPlayerElement.Volume -= 0.05;
                return;
            }

            // Mute/Unmute
            if (e.Key == Key.M || e.Key == Key.VolumeMute)
            {
                MediaPlayerElement.IsMuted = !MediaPlayerElement.IsMuted;
                return;
            }

            // Increase speed
            if (e.Key == Key.Up)
            {
                MediaPlayerElement.SpeedRatio += 0.05;
                return;
            }

            // Decrease speed
            if (e.Key == Key.Down)
            {
                MediaPlayerElement.SpeedRatio -= 0.05;
                return;
            }

            // Cycle through closed captions
            if (e.Key == Key.C)
            {
                var currentCaptions = (int)MediaPlayerElement.ClosedCaptionsChannel;
                var nextCaptions = currentCaptions >= (int)CaptionsChannel.CC4 ? CaptionsChannel.CCP : (CaptionsChannel)(currentCaptions + 1);
                MediaPlayerElement.ClosedCaptionsChannel = nextCaptions;
                return;
            }

            // Reset changes
            if (e.Key == Key.R)
            {
                MediaPlayerElement.SpeedRatio = 1.0;
                MediaPlayerElement.Volume = 1.0;
                MediaPlayerElement.Balance = 0;
                MediaPlayerElement.IsMuted = false;
                ViewModel.Controller.MediaElementZoom = 1.0;
                return;
            }

            // Exit fullscreen
            if (e.Key == Key.Escape)
            {
                ViewModel._applicationState.IsFullScreen = false;
                return;
            }
        }

        /// <summary>
        /// Handles the PreviewMouseDoubleClick event of the Player control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnMediaDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != MediaPlayerElement) return;
            e.Handled = true;
            ViewModel._applicationState.IsFullScreen = !ViewModel._applicationState.IsFullScreen;
        }

        /// <summary>
        /// Handles the MouseWheel event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseWheelEventArgs"/> instance containing the event data.</param>
        private void OnMouseWheelChange(object sender, MouseWheelEventArgs e)
        {
            if (MediaPlayerElement.IsOpen == false || MediaPlayerElement.IsOpening 
                //|| MediaPlayerElement.IsChanging
                )
                return;

            var delta = (e.Delta / 2000d).ToMultipleOf(0.05d);
            ViewModel.Controller.MediaElementZoom = Math.Round(ViewModel.Controller.MediaElementZoom + delta, 2);
        }

        #endregion

        #region Dispose

        protected bool Disposed;

        private void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            Loaded -= OnMediaPlayerLoaded;

            Disposed = true;

            if (disposing) GC.SuppressFinalize(this);
        }

        public void Dispose() => Dispose(true);

        #endregion
    }
}
