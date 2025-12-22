namespace Miru.Domain
{
    public class EpisodeEntity
    {
        /// <summary>
        /// Unique identifier for the episode.
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
    }
}
