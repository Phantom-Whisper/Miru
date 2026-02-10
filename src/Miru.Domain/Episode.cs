namespace Miru.Domain
{
    public class Episode
    {
        /// <summary>
        /// Unique identifier for the episode.
        /// </summary>
        public Guid Id { get; private set; }
        
        /// <summary>
        /// Episode number within the season.
        /// </summary>
        public int EpisodeNumber { get; private set; }

        /// <summary>
        /// Episode title.
        /// </summary>
        public string Title { get; private set; } = null!;

        /// <summary>
        /// Duration of the episode in minutes.
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
        /// Navigation property
        /// </summary>
        public Season Season { get; private set; } = null!;
    }
}
