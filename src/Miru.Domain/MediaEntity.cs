namespace Miru.Domain;

public abstract class MediaEntity
{
    /// <summary>
    /// Unique identifier for the media.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Title of the media.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Description of the media.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Poster URL of the media.
    /// </summary>
    public string PosterUrl { get; set; }
}
