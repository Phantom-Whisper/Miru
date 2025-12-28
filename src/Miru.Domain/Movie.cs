namespace Miru.Domain
{
    public class Movie : Media
    {
        /// <summary>
        /// Duration of the movie in minutes.
        /// </summary>
        public TimeSpan Duration { get; private set; }

        /// <summary>
        /// Default constructor for ORM.
        /// </summary>
        protected Movie() { }

        /// <summary>
        /// Constructor creating a new Movie.
        /// </summary>
        /// <param name="title">Title of the movie.</param>
        /// <param name="releaseDate">Release date.</param>
        /// <param name="duration">Duration of the movie.</param>
        /// <param name="description">Optional description.</param>
        /// <param name="posterUrl">Optional poster URL.</param>
        /// <exception cref="ArgumentException">Thrown if the duration of the movie is less than zero</exception>
        public Movie(string title, DateOnly releaseDate, TimeSpan duration, string? description = null, string? posterUrl = null)
            : base(title, releaseDate, description, posterUrl)
        {
            if (duration <= TimeSpan.Zero)
                throw new ArgumentException("Duration must be greater than zero.", nameof(duration));
            Duration = duration;
        }
    }
}
