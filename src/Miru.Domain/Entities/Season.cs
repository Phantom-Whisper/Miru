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
    
    /// <summary>
    /// Creates a new <see cref="Season"/>.
    /// </summary>
    /// <param name="seasonNumber">Season number (must be positive).</param>
    /// <param name="releaseDate">Release date of the season.</param>
    /// <param name="serieId">Identifier of the parent series.</param>
    /// <exception cref="DomainException">
    /// Thrown when parameters are invalid.
    /// </exception>
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

    /// <summary>
    /// Adds an episode to the season.
    /// </summary>
    /// <param name="episode">Episode to add.</param>
    /// <exception cref="DomainException">
    /// Thrown if the episode is null, does not belong to this season,
    /// or if an episode with the same number already exists.
    /// </exception>
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

    /// <summary>
    /// Updates the release date of the season.
    /// </summary>
    /// <param name="releaseDate">New release date.</param>
    /// <exception cref="DomainException">
    /// Thrown if the release date is in the future.
    /// </exception>
    public void UpdateReleaseDate(DateOnly releaseDate)
    {
        if (releaseDate > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new DomainException("Release date cannot be in the future");

        ReleaseDate = releaseDate;
    }

    /// <summary>
    /// Updates the season number.
    /// </summary>
    /// <param name="seasonNumber">New season number (must be positive).</param>
    /// <exception cref="DomainException">
    /// Thrown if the season number is not positive.
    /// </exception>
    public void UpdateSeasonNumber(int seasonNumber)
    {
        if (seasonNumber <= 0)
            throw new DomainException("Season number must be positive");

        SeasonNumber = seasonNumber;
    }
}