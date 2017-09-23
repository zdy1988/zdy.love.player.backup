using GalaSoft.MvvmLight.Messaging;

namespace Zhu.Messaging
{
    /// <summary>
    /// Used to broadcast a window state change
    /// </summary>
    public class WindowStateChangeMessage : MessageBase
    {
        /// <summary>
        /// Indicated if a movie is playing
        /// </summary>
        public bool IsMoviePlaying { get; }

        /// <summary>
        /// Initialize a new instance of WindowStateChangeMessage class
        /// </summary>
        /// <param name="isMoviePlaying">Indicates if a movie is playing</param>
        public WindowStateChangeMessage(bool isMoviePlaying)
        {
            IsMoviePlaying = isMoviePlaying;
        }
    }
}