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
    }
}
