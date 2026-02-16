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
    /// Factory method.
    /// </summary>
    /// <param name="episodeNumber"></param>
    /// <param name="title"></param>
    /// <param name="duration"></param>
    /// <param name="seasonId"></param>
    /// <returns></returns>
    public static Episode Create(int episodeNumber, string title, TimeSpan duration, Guid seasonId)
    {
        if (episodeNumber <= 0)
            throw new ArgumentException("Episode number must be positive");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required");

        if (duration <= TimeSpan.Zero)
            throw new ArgumentException("Duration must be positive");

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
    /// Marks the episode as watched.
    /// </summary>
    public void MarkAsWatched()
    {
        Watched = true;
        WatchedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Marks the episode as unwatched.
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
    /// <exception cref="ArgumentException">Thrown if the user rating is lower than 0 or higher than 10.</exception>
    public void SetRating(double rating)
    {
        if (rating is < MinRating or > MaxRating)
            throw new ArgumentException("Rating must be between 0 and 10");
        
        Rating = rating;
    }
}