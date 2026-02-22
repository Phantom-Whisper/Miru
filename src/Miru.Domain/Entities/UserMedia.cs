namespace Miru.Domain.Entities;

// May be used in later version
public class UserMedia
{
    private const int MIN_RATING = 0;
    private const int MAX_RATING = 10;
    
    /// <summary>
    /// Identifier of the user.
    /// <remarks> This identifier is a composite key based on UserId and MediaId.</remarks>
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// User associated with the media.
    /// </summary>
    public UserEntity User { get; private set; } = null!;

    /// <summary>
    /// Identifier of the media.
    /// </summary>
    public Guid MediaId { get; private set; }

    /// <summary>
    /// Media associated with the user.
    /// </summary>
    public Media Media { get; private set; } = null!;

    /// <summary>
    /// Current status of the media for the user.
    /// </summary>
    public MediaStatus Status { get; private set; }

    /// <summary>
    /// Date and time when the media was added to the user's collection.
    /// </summary>
    public DateTime AddedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Date and time when the media was watched by the user.
    /// </summary>
    public DateTime? WatchedAt { get; private set; }

    /// <summary>
    /// Rating given by the user to the media.
    /// </summary>
    public double? Rating { get; private set; }

    /// <summary>
    /// Default constructor for ORM.
    /// </summary>
    protected UserMedia() { }
    
    public static UserMedia Create(Guid userId, Guid mediaId, MediaStatus status)
    {
        return new UserMedia
        {
            UserId = userId,
            MediaId = mediaId,
            Status = status,
            AddedAt = DateTime.UtcNow
        };
    }

    public void UpdateStatus(MediaStatus status)
    {
        Status = status;
    }

    public void SetWatchedDate(DateTime watchedAt)
    {
        WatchedAt = watchedAt;
    }

    public void SetRating(double rating)
    {
        if (rating < MIN_RATING || rating > MAX_RATING)
            throw new ArgumentException($"Rating must be between {MIN_RATING} and {MAX_RATING}");
    
        Rating = rating;
    }
}