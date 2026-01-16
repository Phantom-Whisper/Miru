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
    }
}
