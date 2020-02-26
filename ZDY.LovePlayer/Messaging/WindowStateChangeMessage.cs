using GalaSoft.MvvmLight.Messaging;

namespace ZDY.LovePlayer.Messaging
{
    /// <summary>
    /// Used to broadcast a window state change
    /// </summary>
    public class WindowStateChangeMessage : MessageBase
    {
        /// <summary>
        /// Indicated if a movie is playing
        /// </summary>
        public bool IsMediaPlaying { get; }

        /// <summary>
        /// Initialize a new instance of WindowStateChangeMessage class
        /// </summary>
        /// <param name="isMediaPlaying">Indicates if a movie is playing</param>
        public WindowStateChangeMessage(bool isMediaPlaying)
        {
            IsMediaPlaying = isMediaPlaying;
        }
    }
}