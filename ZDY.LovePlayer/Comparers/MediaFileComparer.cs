using System.Collections.Generic;
using ZDY.LovePlayer.Models;

namespace ZDY.LovePlayer.Comparers
{
    /// <summary>
    /// Compare two movies
    /// </summary>
    public class MediaFileComparer : IEqualityComparer<Media>
    {
        /// <summary>
        /// Compare two movies
        /// </summary>
        /// <param name="x">First movie</param>
        /// <param name="y">Second movie</param>
        /// <returns>True if both movies are the same, false otherwise</returns>
        public bool Equals(Media x, Media y)
        {
            //Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            //return x.ID == y.ID && x.MD5 == y.MD5;
            return x.ID == y.ID;
        }

        /// <summary>
        /// Define a unique hash code for a movie
        /// </summary>
        /// <param name="movie">A movie</param>
        /// <returns>Unique hashcode</returns>
        public int GetHashCode(Media movie)
        {
            //Check whether the object is null
            if (ReferenceEquals(movie, null)) return 0;

            //Get hash code for the Id field
            var hashId = movie.ID.GetHashCode();

            //Get hash code for the MD5 field.
            //var hashDate = movie.MD5.GetHashCode();

            //return hashId ^ hashDate;
            return hashId;
        }
    }
}