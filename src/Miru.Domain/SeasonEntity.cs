namespace Miru.Domain
{
    public class SeasonEntity
    {
        /// <summary>
        /// Unique identifier for the season.
        /// <remarks>This identifier is defined by the SerieId and the SeasonNumber</remarks>
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Season number within the serie.
        /// </summary>
        public int SeasonNumber { get; set; }

        /// <summary>
        /// Date when the season was released.
        /// </summary>
        public DateOnly ReleaseDate { get; set; }

        /// <summary>
        /// Identifier of the serie this season belongs to.
        /// </summary>
        public Guid SerieId { get; set; }

        /// <summary>
        /// List of episodes in the season.
        /// </summary>
        public ICollection<EpisodeEntity> Episodes { get; set; } = [];
    }
}
