using Miru.Domain.Exceptions;

namespace Miru.Domain.Entities;

public class Season
{
    /// <summary>
    /// Unique identifier for the season.
    /// </summary>
    public Guid Id { get; private set; }
    
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
    
    public static Season Create(int seasonNumber, DateOnly releaseDate, Guid serieId)
    {
        if (seasonNumber <= 0)
            throw new DomainException("Season number must be positive");

        if (serieId == Guid.Empty)
            throw new DomainException("SerieId cannot be empty");

        return new Season
        {
            Id = Guid.NewGuid(),
            SeasonNumber = seasonNumber,
            ReleaseDate = releaseDate,
            SerieId = serieId
        };
    }

    public void AddEpisode(Episode episode)
    {
        if (episode == null)
            throw new DomainException(nameof(episode));

        if (episode.SeasonId != Id)
            throw new DomainException("Episode does not belong to this season");

        if (_episodes.Any(e => e.EpisodeNumber == episode.EpisodeNumber))
            throw new DomainException($"Episode {episode.EpisodeNumber} already exists in this season");

        _episodes.Add(episode);
    }
    
    /// <summary>
    /// Navigation property
    /// </summary>
    public Serie Serie { get; private set; } = null!;
}