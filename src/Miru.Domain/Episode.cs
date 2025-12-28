namespace Miru.Domain
{
    public class Episode
    {
        /// <summary>
        /// Episode number within the season.
        /// </summary>
        public int EpisodeNumber { get; private set; }

        /// <summary>
        /// Episode title.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Dureation of the episode in minutes.
        /// </summary>
        public TimeSpan Duration { get; private set; }

        /// <summary>
        /// Identifier of the season this episode belongs to.
        /// </summary>
        public Guid SeasonId { get; private set; }

        /// <summary>
        /// Default constructor for ORM.
        /// </summary>
        protected Episode() { }

        /// <summary>
        /// Constructor creating a new Episode.
        /// </summary>
        /// <param name="episodeNumber">Episode number.</param>
        /// <param name="title">Title of the episode.</param>
        /// <param name="duration">Duration of the episode.</param>
        /// <param name="seasonId">Season identifier.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the episode number is lower than zero.</exception>
        /// <exception cref="ArgumentException">Thrown if title is null or empty, or if duration is less than zero.</exception>
        /// <exception cref="ArgumentException">Thrown if duration is less than or equal to zero.</exception>
        public Episode(int episodeNumber, string title, TimeSpan duration, Guid seasonId)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(episodeNumber);
            if (string.IsNullOrWhiteSpace(title)) 
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));
            if (duration <= TimeSpan.Zero) 
                throw new ArgumentException("Duration must be positive.", nameof(duration));

            EpisodeNumber = episodeNumber;
            Title = title;
            Duration = duration;
            SeasonId = seasonId;
        }
    }
}
