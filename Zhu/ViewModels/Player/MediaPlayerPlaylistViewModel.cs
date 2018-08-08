using Unosquare.FFME.Events;
using Zhu.Foundation;
using Unosquare.FFME.Platform;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using Zhu.Untils;
using GalaSoft.MvvmLight;

namespace Zhu.ViewModels.Player
{
    /// <summary>
    /// Represents the Playlist
    /// </summary>
    /// <seealso cref="AttachedViewModel" />
    public sealed class MediaPlayerPlaylistViewModel : ViewModelBase
    {
        // Constants
        private const int MinimumSearchLength = 3;

        // Private state management
        private readonly TimeSpan SearchActionDelay = TimeSpan.FromSeconds(0.25);
        private bool HasTakenThumbnail = false;
        private DeferredAction SearchAction = null;
        private string FilterString = string.Empty;

        private MediaPlayerViewModel _mediaPlayer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistViewModel"/> class.
        /// </summary>
        /// <param name="root">The root.</param>
        public MediaPlayerPlaylistViewModel(MediaPlayerViewModel mediaPlayer)
        {
            _mediaPlayer = mediaPlayer;

            ThumbsDirectory = Path.Combine(Constants.ImageTempDirectory, "Thumbnails");
            if (Directory.Exists(ThumbsDirectory) == false)
                Directory.CreateDirectory(ThumbsDirectory);

            PlaylistFilePath = Path.Combine(Constants.ImageTempDirectory, "ffme.m3u8");

            // Set and create a thumbnails directory
            Entries = new CustomPlaylist(this);
            EntriesView = CollectionViewSource.GetDefaultView(Entries) as ICollectionView;
            EntriesView.Filter = (item) =>
            {
                if (item is CustomPlaylistEntry == false) return false;
                var entry = item as CustomPlaylistEntry;

                if (string.IsNullOrWhiteSpace(PlaylistSearchString) || PlaylistSearchString.Trim().Length < MinimumSearchLength)
                    return true;

                if ((entry.Title?.ToLowerInvariant().Contains(PlaylistSearchString) ?? false) ||
                    (entry.MediaUrl?.ToLowerInvariant().Contains(PlaylistSearchString) ?? false))
                    return true;

                return false;
            };

            RaisePropertyChanged(nameof(EntriesView));
        }

        /// <summary>
        /// Gets the custom playlist. Do not use for data-binding
        /// </summary>
        public CustomPlaylist Entries { get; set; }

        /// <summary>
        /// Gets the custom playlist entries as a view that can be uased in data binding scenarios.
        /// </summary>
        public ICollectionView EntriesView { get; private set; }

        /// <summary>
        /// Gets the full path wehre thumbnails are stored.
        /// </summary>
        public string ThumbsDirectory { get; }

        /// <summary>
        /// Gets the playlist file path.
        /// </summary>
        public string PlaylistFilePath { get; }

        private string _playlistSearchString = string.Empty;

        /// <summary>
        /// Gets or sets the playlist search string.
        /// </summary>
        public string PlaylistSearchString
        {
            get => _playlistSearchString;
            set
            {
                if (!Set(() => PlaylistSearchString, ref _playlistSearchString, value))
                    return;

                if (SearchAction == null)
                {
                    SearchAction = DeferredAction.Create(() =>
                    {
                        var futureSearch = PlaylistSearchString ?? string.Empty;
                        var currentSearch = FilterString ?? string.Empty;

                        if (currentSearch == futureSearch) return;
                        if (futureSearch.Length < MinimumSearchLength && currentSearch.Length < MinimumSearchLength) return;

                        EntriesView.Refresh();
                        FilterString = string.Copy(_playlistSearchString) ?? string.Empty;
                    });
                }

                SearchAction.Defer(SearchActionDelay);
            }
        }

        private bool _isPlaylistEnabled = true;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is playlist enabled.
        /// </summary>
        public bool IsPlaylistEnabled
        {
            get { return _isPlaylistEnabled; }
            set { Set(() => IsPlaylistEnabled, ref _isPlaylistEnabled, value); }
        }

        private bool _isInOpenMode = GuiContext.Current.IsInDesignTime;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in open mode.
        /// </summary>
        public bool IsInOpenMode
        {
            get { return _isInOpenMode; }
            set { Set(() => IsInOpenMode, ref _isInOpenMode, value); }
        }

        private string _openTargetUrl = string.Empty;

        /// <summary>
        /// Gets or sets the open model URL.
        /// </summary>
        public string OpenTargetUrl
        {
            get { return _openTargetUrl; }
            set { Set(() => OpenTargetUrl, ref _openTargetUrl, value); }
        }

        /// <summary>
        /// Called by the root ViewModel when the application is loaded and fully available
        /// </summary>
        public void MediaPlayerLoaded()
        {
            var m = _mediaPlayer.MediaElement;

            new Action(() =>
            {
                IsPlaylistEnabled = m.IsOpening == false;
            }).WhenChanged(m, nameof(m.IsOpening));

            m.MediaOpened += OnMediaOpened;
            m.RenderingVideo += OnRenderingVideo;
        }

        /// <summary>
        /// Called when Media is opened
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnMediaOpened(object sender, System.Windows.RoutedEventArgs e)
        {
            HasTakenThumbnail = false;
            Entries.AddOrUpdateEntry(
                _mediaPlayer.MediaElement.Source?.ToString() ?? _mediaPlayer.MediaElement.MediaInfo.InputUrl,
                _mediaPlayer.MediaElement.MediaInfo);
            Entries.SaveEntries();
        }

        /// <summary>
        /// Handles the RenderingVideo event of the Media control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RenderingVideoEventArgs"/> instance containing the event data.</param>
        private void OnRenderingVideo(object sender, RenderingVideoEventArgs e)
        {
            const double snapPosition = 3;

            var state = e.EngineState;
            if (HasTakenThumbnail || state.Source == null)
                return;

            var sourceUrl = state.Source.ToString();
            if (string.IsNullOrWhiteSpace(sourceUrl))
                return;

            if (state.HasMediaEnded
                || state.Position.TotalSeconds >= snapPosition
                || (state.NaturalDuration.HasValue && state.NaturalDuration.Value.TotalSeconds <= snapPosition))
            {
                HasTakenThumbnail = true;
                Entries.AddOrUpdateEntryThumbnail(sourceUrl, e.Bitmap);
                Entries.SaveEntries();
            }
        }
    }
}
