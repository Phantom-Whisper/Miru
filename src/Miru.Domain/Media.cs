namespace Miru.Domain;

public abstract class Media
{
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
    public DateOnly ReleaseDate { get; private set; }

    /// <summary>
    /// List of media associated with user.
    /// </summary>
    private readonly List<UserMedia> _userMedias = [];
    public IReadOnlyCollection<UserMedia> UserMedias => _userMedias.AsReadOnly();

    /// <summary>
    /// Default constructor for ORM.
    /// </summary>
    protected Media() { }
}
