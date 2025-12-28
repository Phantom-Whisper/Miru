namespace Miru.Domain
{
    public class Serie : Media
    {
        /// <summary>
        /// List of seasons in the serie.
        /// </summary>
        private readonly List<Season> _seasons = [];
        public IReadOnlyCollection<Season> Seasons => _seasons.AsReadOnly();

        /// <summary>
        /// Default constructor for ORM.
        /// </summary>
        protected Serie() { }

        /// <summary>
        /// Constructor creating a new Serie.
        /// </summary>
        /// <param name="title">Title of the serie.</param>
        /// <param name="releaseDate">Release date.</param>
        /// <param name="description">Optional description.</param>
        /// <param name="posterUrl">Optional poster URL.</param>
        public Serie(string title, DateOnly releaseDate, string? description = null, string? posterUrl = null)
            : base(title, releaseDate, description, posterUrl)
        {
        }

        /// <summary>
        /// Adds a season to the serie.
        /// </summary>
        /// <param name="season">The season to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if the season's null</exception>
        /// <exception cref="InvalidOperationException">Thrown if a season with the same season number already exists.</exception>
        internal void AddSeason(Season season)
        {
            if (season == null) 
                throw new ArgumentNullException(nameof(season), "Season cannot be null.");
            if (_seasons.Any(s => s.SeasonNumber == season.SeasonNumber))
                throw new InvalidOperationException("This season number already exists.");
            _seasons.Add(season);
        }

        /// <summary>
        /// Removes a season from the serie.
        /// </summary>
        /// <param name="season">The season to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if the season's null</exception>
        internal void RemoveSeason(Season season)
        {
            if (season == null) 
                throw new ArgumentNullException(nameof(season), "Season cannot be null.");
            _seasons.Remove(season);
        }
    }
}
