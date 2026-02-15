using Miru.Domain.Exceptions;

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
        /// Factory method.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="title"></param>
        /// <param name="releaseDate"></param>
        /// <param name="description"></param>
        /// <param name="posterUrl"></param>
        /// <returns></returns>
        public static Serie Create(Guid userId, string title, DateOnly releaseDate, string? description = null, string? posterUrl = null)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty");
    
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Title is required");
            
            return new Serie
            {
                Id = Guid.NewGuid(),
                UserId =  userId,
                Title = title,
                ReleaseDate = releaseDate,
                Description = description,
                PosterUrl = posterUrl
            };
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="season"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void AddSeason(Season season)
        {
            if (season == null)
                throw new DomainException(nameof(season));
    
            if (season.SerieId != Id)
                throw new DomainException("Season does not belong to this serie");
    
            if (_seasons.Any(s => s.SeasonNumber == season.SeasonNumber))
                throw new DomainException($"Season {season.SeasonNumber} already exists");

            _seasons.Add(season);
        }
    }
}
