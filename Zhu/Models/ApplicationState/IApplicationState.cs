namespace Zhu.Models.ApplicationState
{
    public interface IApplicationState
    {
        /// <summary>
        /// Indicates if application is fullscreen
        /// </summary>
        bool IsFullScreen { get; set; }

        /// <summary>
        /// Specify if a connection error has occured
        /// </summary>
        bool IsConnectionInError { get; set; }

        /// <summary>
        /// Specify if a movie is playing
        /// </summary>
        bool IsMoviePlaying { get; set; }
    }
}