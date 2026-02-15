namespace Miru.Domain;

public abstract class Media
{
    private const double MinRating = 0;
    private const double MaxRating = 10;
    
    /// <summary>
    /// Unique identifier for the media.
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// Title of the media.
    /// </summary>
    public string Title { get; protected set; } = null!;

    /// <summary>
    /// Description of the media.
    /// </summary>
    public string? Description { get; protected set; }

    /// <summary>
    /// Poster URL of the media.
    /// </summary>
    public string? PosterUrl { get; protected set; }

    /// <summary>
    /// Release date of the media.
    /// </summary>
    public DateOnly ReleaseDate { get; protected set; }
    
    /// <summary>
    /// Current status of the media.
    /// </summary>
    public MediaStatus Status { get; protected set; } = MediaStatus.ToWatch;
    
    /// <summary>
    /// Date the media was added by the user.
    /// </summary>
    public DateTime AddedAt { get; protected set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date the media was watched by the user.
    /// </summary>
    public DateTime? WatchedAt { get; protected set; }
    
    /// <summary>
    /// Rating given by the user to the media.
    /// </summary>
    public double? Rating { get; protected set; }
    
    /// <summary>
    /// Default constructor for ORM.
    /// </summary>
    protected Media() { }
    
    // Navigation properties
    /// <summary>
    /// Associated user id.
    /// </summary>
    public Guid UserId { get; protected set; }
    
    /// <summary>
    /// Associated user.
    /// </summary>
    public UserEntity User { get; protected set; } = null!;
    
    // Utility methods
    
    /// <summary>
    /// Updates media status.
    /// </summary>
    /// <param name="newStatus">New status given to the media.</param>
    public void UpdateStatus(MediaStatus newStatus)
    {
        Status = newStatus;
        if (newStatus == MediaStatus.Watched && WatchedAt == null)
        {
            WatchedAt = DateTime.UtcNow;
        }
    }
    
    /// <summary>
    /// Sets the user rating of the media.
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