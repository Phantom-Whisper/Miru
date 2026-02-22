using Miru.Domain.Exceptions;

namespace Miru.Domain.Entities;

public class Serie : Media
{
    /// <summary>
    /// List of seasons in the serie.
    /// </summary>
    private readonly List<Season> _seasons = [];
    
    /// <summary>
    /// Gets the read-only collection of seasons.
    /// </summary>
    public IReadOnlyCollection<Season> Seasons => _seasons.AsReadOnly();

    /// <summary>
    /// Default constructor for ORM.
    /// </summary>
    protected Serie() { }
    
    /// <summary>
    /// Creates a new <see cref="Serie"/> instance.
    /// </summary>
    /// <param name="userId">Identifier of the owner user.</param>
    /// <param name="title">Title of the series.</param>
    /// <param name="releaseDate">Initial release date.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="posterUrl">Optional poster URL.</param>
    /// <returns>A new valid <see cref="Serie"/> instance.</returns>
    /// <exception cref="DomainException">
    /// Thrown when required parameters are invalid.
    /// </exception>
    public static Serie Create(Guid userId, string title, DateOnly releaseDate, string? description = null, string? posterUrl = null)
    {
        if (userId == Guid.Empty)
            throw new DomainException("UserId cannot be empty");

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
    /// Adds a season to the series.
    /// </summary>
    /// <param name="season">The season to add.</param>
    /// <exception cref="DomainException">
    /// Thrown when the season is null,
    /// does not belong to this series,
    /// or a season with the same number already exists.
    /// </exception>
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

    /// <summary>
    /// Updates the title of the series.
    /// </summary>
    /// <param name="title">The new title.</param>
    /// <exception cref="DomainException">
    /// Thrown when the title is null, empty, or whitespace.
    /// </exception>
    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title is required");

        Title = title;
    }

    /// <summary>
    /// Updates the release date of the series.
    /// </summary>
    /// <param name="releaseDate">The new release date.</param>
    /// <exception cref="DomainException">
    /// Thrown when the release date is in the future.
    /// </exception>
    public void UpdateReleaseDate(DateOnly releaseDate)
    {
        if (releaseDate > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new DomainException("Release date cannot be in the future");

        ReleaseDate = releaseDate;
    }

    /// <summary>
    /// Updates the description of the series.
    /// </summary>
    /// <param name="description">
    /// The new description. Can be null to remove the current description.
    /// </param>
    public void UpdateDescription(string description)
    {
        Description = description;
    }

    /// <summary>
    /// Updates the poster URL of the series.
    /// </summary>
    /// <param name="posterUrl">
    /// The new poster URL. Cannot be null or whitespace.
    /// </param>
    /// <exception cref="DomainException">
    /// Thrown when the poster URL is empty or whitespace.
    /// </exception>
    public void UpdatePosterUrl(string posterUrl)
    {
        if (string.IsNullOrWhiteSpace(posterUrl))
            throw new DomainException("Poster URL cannot be empty");

        PosterUrl = posterUrl;
    }
}