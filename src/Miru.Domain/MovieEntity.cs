namespace Miru.Domain
{
    public class MovieEntity : MediaEntity
    {
        /// <summary>
        /// Release date of the movie.
        /// </summary>
        public DateOnly ReleaseDate { get; set; }

        /// <summary>
        /// Duration of the movie in minutes.
        /// </summary>
        public long Duration { get; set; }
    }
}
