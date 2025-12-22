namespace Miru.Domain
{
    public class EpisodeEntity
    {
        /// <summary>
        /// Unique identifier for the episode.
        /// <remarks>This identifier is defined on SeasonId and EpisodeNumber</remarks>
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Episode number within the season.
        /// </summary>
        public int EpisodeNumber { get; set; }

        /// <summary>
        /// Episode title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Dureation of the episode in minutes.
        /// </summary>
        public long Duration { get; set; }

        /// <summary>
        /// Identifier of the season this episode belongs to.
        /// </summary>
        public Guid SeasonId { get; set; }
    }
}
