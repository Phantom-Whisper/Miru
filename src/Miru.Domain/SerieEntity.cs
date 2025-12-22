namespace Miru.Domain
{
    public class SerieEntity : MediaEntity
    {
        /// <summary>
        /// List of seasons in the serie.
        /// </summary>
        public ICollection<SeasonEntity> Seasons { get; set; } = [];
    }
}
