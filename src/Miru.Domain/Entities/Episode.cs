using Miru.Domain.Exceptions;

namespace Miru.Domain.Entities;

public class Episode
{
    private const double MinRating = 0;
    private const double MaxRating = 10;
    
    /// <summary>
    /// Unique identifier for the episode.
    /// </summary>
    public Guid Id { get; private set; }
    
    /// <summary>
    /// Episode number within the season.
    /// </summary>
    public int EpisodeNumber { get; private set; }

    /// <summary>
    /// Episode title.
    /// </summary>
    public string Title { get; private set; } = null!;

    /// <summary>
    /// Duration of the episode in minutes.
    /// </summary>
    public TimeSpan Duration { get; private set; }

    /// <summary>
    /// Identifier of the season this episode belongs to.
    /// </summary>
    public Guid SeasonId { get; private set; }
    
    /// <summary>
    /// Navigation property
    /// </summary>
    public Season Season { get; private set; } = null!;
    
    /// <summary>
    /// Tells if the episode's watched.
    /// </summary>
    public bool Watched { get; private set; } = false;
    
    /// <summary>
    /// Date the episode was watched by the user.
    /// </summary>
    public DateTime? WatchedAt { get; private set; }
    
    /// <summary>
    /// Rating given by the user to the episode.
    /// </summary>
    public double? Rating { get; private set; }

    /// <summary>
    /// Default constructor for ORM.
    /// </summary>
    protected Episode() { }
    
    /// <summary>
    /// Creates a new <see cref="Episode"/>.
    /// </summary>
    /// <exception cref="DomainException">
    /// Thrown when parameters are invalid.
    /// </exception>
    public static Episode Create(int episodeNumber, string title, TimeSpan duration, Guid seasonId)
    {
        if (episodeNumber <= 0)
            throw new DomainException("Episode number must be positive");

        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title is required");

        if (duration <= TimeSpan.Zero)
            throw new DomainException("Duration must be positive");

        if (seasonId == Guid.Empty)
            throw new DomainException("SeasonId cannot be empty");

        return new Episode
        {
            Id = Guid.NewGuid(),
            EpisodeNumber = episodeNumber,
            Title = title,
            Duration = duration,
            SeasonId = seasonId
        };
    }
    
    // Utility methods
    
    /// <summary>
    /// Marks the episode as watched and sets the watched date to now (UTC).
    /// </summary>
    public void MarkAsWatched()
    {
        Watched = true;
        WatchedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Marks the episode as not watched and clears the watched date.
    /// </summary>
    public void MarkAsUnwatched()
    {
        Watched = false;
        WatchedAt = null;
    }

    /// <summary>
    /// Sets the episode rating given by the user.
    /// </summary>
    /// <param name="rating">User rating.</param>
    /// <exception cref="DomainException">Thrown if the user rating is lower than 0 or higher than 10.</exception>
    public void SetRating(double rating)
    {
        if (rating is < MinRating or > MaxRating)
            throw new DomainException("Rating must be between 0 and 10");
        
        Rating = rating;
    }

    /// <summary>
    /// Updates the episode duration.
    /// </summary>
    /// <exception cref="DomainException">
    /// Thrown if duration is not positive.
    /// </exception>
    public void UpdateDuration(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
            throw new DomainException("Duration must be positive");

        Duration = duration;
    }

    /// <summary>
    /// Updates the episode title.
    /// </summary>
    /// <exception cref="DomainException">
    /// Thrown when title is null or empty.
    /// </exception>
    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title is required");

        Title = title;
    }

    /// <summary>
    /// Updates the episode number.
    /// </summary>
    /// <exception cref="DomainException">
    /// Thrown when episode number is not positive.
    /// </exception>
    public void UpdateEpisodeNumber(int episodeNumber)
    {
        if (episodeNumber <= 0)
            throw new DomainException("Episode number must be positive");

        EpisodeNumber = episodeNumber;
    }
}