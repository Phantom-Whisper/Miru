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
        public string Title { get; private set; } = null!;

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
    }
}
