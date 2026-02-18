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
}