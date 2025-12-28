namespace Miru.Domain
{
    public class Season
    {
        /// <summary>
        /// Season number within the serie.
        /// </summary>
        public int SeasonNumber { get; private set; }

        /// <summary>
        /// Date when the season was released.
        /// </summary>
        public DateOnly ReleaseDate { get; private set; }

        /// <summary>
        /// Identifier of the serie this season belongs to.
        /// </summary>
        public Guid SerieId { get; private set; }

        /// <summary>
        /// List of episodes in the season.
        /// </summary>
        private readonly List<Episode> _episodes = [];
        public IReadOnlyCollection<Episode> Episodes => _episodes.AsReadOnly();

        /// <summary>
        /// Default constructor for ORM.
        /// </summary>
        protected Season() { }

        /// <summary>
        /// Constructor creating a new Season.
        /// </summary>
        /// <param name="seasonNumber">Number of the season.</param>
        /// <param name="releaseDate">Date of release.</param>
        /// <param name="serieId">Identifier of the serie.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the season number is lower than zero.</exception>
        public Season(int seasonNumber, DateOnly releaseDate, Guid serieId)
        {
            if (seasonNumber <= 0)
                throw new ArgumentOutOfRangeException(nameof(seasonNumber), "Season number must be greater than zero.");
            SeasonNumber = seasonNumber;
            ReleaseDate = releaseDate;
            SerieId = serieId;
        }

        /// <summary>
        /// Adds a specific episode to the season.
        /// </summary>
        /// <param name="episode">The episode to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if the episode's null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if an episode with the same episode number already exists in this season.</exception>"
        internal void AddEpisode(Episode episode)
        {
            if (episode == null)
                throw new ArgumentNullException(nameof(episode), "Episode cannot be null.");
            if (_episodes.Any(e => e.EpisodeNumber == episode.EpisodeNumber))
                throw new InvalidOperationException("An episode with the same episode number already exists in this season.");
            _episodes.Add(episode);
        }

        /// <summary>
        /// Removes a specific episode from the season.
        /// </summary>
        /// <param name="episode">The episode to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if the episode's null.</exception>
        internal void RemoveEpisode(Episode episode)
        {
            if (episode == null) 
                throw new ArgumentNullException(nameof(episode), "Episode cannot be null.");
            _episodes.Remove(episode);
        }
    }
}
