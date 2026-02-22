using Miru.Domain.Exceptions;

namespace Miru.Domain.Entities;

public class Movie : Media
{
    /// <summary>
    /// Duration of the movie in minutes.
    /// </summary>
    public TimeSpan Duration { get; private set; }

    /// <summary>
    /// Default constructor for ORM.
    /// </summary>
    protected Movie() { }
    
    /// <summary>
    /// Factory method.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="title"></param>
    /// <param name="duration"></param>
    /// <param name="releaseDate"></param>
    /// <param name="description"></param>
    /// <param name="posterUrl"></param>
    /// <returns></returns>
    public static Movie Create(Guid userId, string title, TimeSpan duration, DateOnly releaseDate, string? description = null, string? posterUrl = null)
    {
        if (userId == Guid.Empty)
            throw new DomainException("UserId cannot be empty");

        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title is required");

        if (duration <= TimeSpan.Zero)
            throw new DomainException("Duration must be positive");

        return new Movie
        {
            Id = Guid.NewGuid(),
            UserId =  userId,
            Title = title,
            Duration = duration,
            ReleaseDate = releaseDate,
            Description = description,
            PosterUrl = posterUrl
        };
    }
    
    /// <summary>
    /// Updates the movie title.
    /// </summary>
    /// <param name="title">The new title of the movie.</param>
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
    /// Updates the movie duration.
    /// </summary>
    /// <param name="duration">The new duration of the movie.</param>
    /// <exception cref="DomainException">
    /// Thrown when the duration is less than or equal to zero.
    /// </exception>
    public void UpdateDuration(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
            throw new DomainException("Duration must be positive");

        Duration = duration;
    }

    /// <summary>
    /// Updates the release date of the movie.
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
    /// Updates the movie description.
    /// </summary>
    /// <param name="description">
    /// The new description. Can be null to remove the current description.
    /// </param>
    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    /// <summary>
    /// Updates the poster URL of the movie.
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