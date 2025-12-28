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
    public string Title { get; protected set; }

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

    /// <summary>
    /// Constructor creating a new Media.
    /// </summary>
    /// <param name="title">Title of the media.</param>
    /// <param name="releaseDate">Release date.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="posterUrl">Optional poster URL.</param>
    /// <exception cref="ArgumentException">Thrown if title is null or empty.</exception>
    public Media(string title, DateOnly releaseDate, string? description = null, string? posterUrl = null)
    {
        if (string.IsNullOrWhiteSpace(title)) 
            throw new ArgumentException("Title cannot be null or empty.", nameof(title));

        Title = title;
        Description = description;
        PosterUrl = posterUrl;
        ReleaseDate = releaseDate;
    }

    /// <summary>
    /// Adds a specific user media to the media.
    /// </summary>
    /// <param name="userMedia">The media to add.</param>
    /// <exception cref="ArgumentNullException">Thrown if the media's null</exception>
    internal void AddUserMedia(UserMedia userMedia)
    {
        if (userMedia == null) 
            throw new ArgumentNullException(nameof(userMedia), "The media cannot be null.");
        _userMedias.Add(userMedia);
    }

    /// <summary>
    /// Removes a specific user media to the media.
    /// </summary>
    /// <param name="userMedia">The media to remove.</param>
    /// <exception cref="ArgumentNullException">Thrown if the media's null</exception>
    internal void RemoveUserMedia(UserMedia userMedia)
    {
        if (userMedia == null) 
            throw new ArgumentNullException(nameof(userMedia), "The media cannot be null.");
        _userMedias.Remove(userMedia);
    }
}
