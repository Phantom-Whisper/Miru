namespace Miru.Domain
{
    public class SeasonEntity
    {
        /// <summary>
        /// Unique identifier for the season.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Season number within the serie.
        /// </summary>
        public int SeasonNumber { get; set; }

        /// <summary>
        /// List of episodes in the season.
        /// </summary>
        public ICollection<EpisodeEntity> Episodes { get; set; } = new List<EpisodeEntity>();
    }
}
