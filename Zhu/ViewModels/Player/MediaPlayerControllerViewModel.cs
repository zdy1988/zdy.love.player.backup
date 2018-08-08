using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Zhu.Foundation;
using Zhu.Services;

namespace Zhu.ViewModels.Player
{
    public class MediaPlayerControllerViewModel : ViewModelBase
    {
        private MediaPlayerViewModel _mediaPlayer;

        public MediaPlayerControllerViewModel(MediaPlayerViewModel mediaPlayer)
        {
            _mediaPlayer = mediaPlayer;
        }

        private Visibility _isMediaOpenVisibility = Visibility.Visible;

        public Visibility IsMediaOpenVisibility
        {
            get { return _isMediaOpenVisibility; }
            set { Set(() => IsMediaOpenVisibility, ref _isMediaOpenVisibility, value); }
        }

        private bool _isAudioControlEnabled = true;

        public bool IsAudioControlEnabled
        {
            get { return _isAudioControlEnabled; }
            set { Set(() => IsAudioControlEnabled, ref _isAudioControlEnabled, value); }
        }

        private bool _isSpeedRatioEnabled = true;

        public bool IsSpeedRatioEnabled
        {
            get { return _isSpeedRatioEnabled; }
            set { Set(() => IsSpeedRatioEnabled, ref _isSpeedRatioEnabled, value); }
        }

        private Visibility _closedCaptionsVisibility = Visibility.Visible;

        public Visibility ClosedCaptionsVisibility
        {
            get { return _closedCaptionsVisibility; }
            set { Set(() => ClosedCaptionsVisibility, ref _closedCaptionsVisibility, value); }
        }

        private Visibility _audioControlVisibility = Visibility.Visible;

        public Visibility AudioControlVisibility
        {
            get { return _audioControlVisibility; }
            set { Set(() => AudioControlVisibility, ref _audioControlVisibility, value); }
        }

        private Visibility _pauseButtonVisibility = Visibility.Visible;

        public Visibility PauseButtonVisibility
        {
            get { return _pauseButtonVisibility; }
            set { Set(() => PauseButtonVisibility, ref _pauseButtonVisibility, value); }
        }

        private Visibility _playButtonVisibility = Visibility.Visible;

        public Visibility PlayButtonVisibility
        {
            get { return _playButtonVisibility; }
            set { Set(() => PlayButtonVisibility, ref _playButtonVisibility, value); }
        }

        private Visibility _stopButtonVisibility = Visibility.Visible;

        public Visibility StopButtonVisibility
        {
            get { return _stopButtonVisibility; }
            set { Set(() => StopButtonVisibility, ref _stopButtonVisibility, value); }
        }

        private Visibility _openButtonVisibility = Visibility.Visible;

        public Visibility OpenButtonVisibility
        {
            get { return _openButtonVisibility; }
            set { Set(() => OpenButtonVisibility, ref _openButtonVisibility, value); }
        }

        private Visibility _seekBarVisibility = Visibility.Visible;

        public Visibility SeekBarVisibility
        {
            get { return _seekBarVisibility; }
            set { Set(() => SeekBarVisibility, ref _seekBarVisibility, value); }
        }

        private Visibility _bufferingProgressVisibility = Visibility.Visible;

        public Visibility BufferingProgressVisibility
        {
            get { return _bufferingProgressVisibility; }
            set { Set(() => BufferingProgressVisibility, ref _bufferingProgressVisibility, value); }
        }

        private Visibility _downloadProgressVisibility = Visibility.Visible;

        public Visibility DownloadProgressVisibility
        {
            get { return _downloadProgressVisibility; }
            set { Set(() => DownloadProgressVisibility, ref _downloadProgressVisibility, value); }
        }

        /// <summary>
        /// Gets or sets the media element zoom.
        /// </summary>
        public double MediaElementZoom
        {
            get
            {
                var m = _mediaPlayer.MediaElement;
                if (m == null) return 1d;

                var transform = m.RenderTransform as ScaleTransform;
                return transform?.ScaleX ?? 1d;
            }
            set
            {
                var m = _mediaPlayer.MediaElement;
                if (m == null) return;

                var transform = m.RenderTransform as ScaleTransform;
                if (transform == null)
                {
                    transform = new ScaleTransform(1, 1);
                    m.RenderTransformOrigin = new Point(0.5, 0.5);
                    m.RenderTransform = transform;
                }

                transform.ScaleX = value;
                transform.ScaleY = value;

                if (transform.ScaleX < 0.1d || transform.ScaleY < 0.1)
                {
                    transform.ScaleX = 0.1d;
                    transform.ScaleY = 0.1d;
                }
                else if (transform.ScaleX > 5d || transform.ScaleY > 5)
                {
                    transform.ScaleX = 5;
                    transform.ScaleY = 5;
                }


                //RaisePropertyChanged();
            }
        }

        public void MediaPlayerLoaded()
        {
            var m = _mediaPlayer.MediaElement;

            new Action(() => { IsMediaOpenVisibility = m.IsOpen ? Visibility.Visible : Visibility.Hidden; })
                .WhenChanged(m, nameof(m.IsOpen));

            new Action(() => { ClosedCaptionsVisibility = m.HasClosedCaptions ? Visibility.Visible : Visibility.Hidden; })
                .WhenChanged(m, nameof(m.HasClosedCaptions));

            new Action(() =>
            {
                AudioControlVisibility = m.HasAudio ? Visibility.Visible : Visibility.Hidden;
                IsAudioControlEnabled = m.HasAudio;
            }).WhenChanged(m, nameof(m.HasAudio));

            new Action(() => PauseButtonVisibility = m.CanPause && m.IsPlaying ? Visibility.Visible : Visibility.Collapsed).WhenChanged(m, nameof(m.CanPause), nameof(m.IsPlaying));

            new Action(() => PlayButtonVisibility = m.IsPlaying == false ? Visibility.Visible : Visibility.Collapsed).WhenChanged(m, nameof(m.IsPlaying));

            new Action(() =>
            {
                StopButtonVisibility = m.IsOpen && m.IsSeeking == false && (m.HasMediaEnded || (m.IsSeekable && m.MediaState != MediaState.Stop)) ? Visibility.Visible : Visibility.Hidden;
            }).WhenChanged(m, nameof(m.IsOpen), nameof(m.HasMediaEnded), nameof(m.IsSeekable), nameof(m.MediaState), nameof(m.IsSeeking));

            new Action(() => { SeekBarVisibility = m.IsSeekable ? Visibility.Visible : Visibility.Hidden; })
                .WhenChanged(m, nameof(m.IsSeekable));

            new Action(() => { SeekBarVisibility = m.IsSeekable ? Visibility.Visible : Visibility.Hidden; })
                .WhenChanged(m, nameof(m.IsSeekable));

            new Action(() => { BufferingProgressVisibility = m.IsOpening || m.IsBuffering ? Visibility.Visible : Visibility.Hidden; })
                .WhenChanged(m, nameof(m.IsOpening), nameof(m.IsBuffering));

            new Action(() => { DownloadProgressVisibility = m.IsOpen && m.HasMediaEnded == false && ((m.DownloadProgress > 0d && m.DownloadProgress < 0.95) || m.IsLiveStream) ? Visibility.Visible : Visibility.Hidden; })
                .WhenChanged(m, nameof(m.IsOpen), nameof(m.HasMediaEnded), nameof(m.DownloadProgress), nameof(m.IsLiveStream));

            new Action(() => { OpenButtonVisibility = m.IsOpening == false ? Visibility.Visible : Visibility.Hidden; })
                .WhenChanged(m, nameof(m.IsOpening));

            new Action(() => { IsSpeedRatioEnabled = m.IsOpening == false; })
                .WhenChanged(m, nameof(m.IsOpen), nameof(m.IsSeekable));
        }
    }
}
