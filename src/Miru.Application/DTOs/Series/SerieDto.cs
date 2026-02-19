namespace Miru.Application.DTOs.Series;

/// <summary>
/// Lightweight serie information for lists.
/// </summary>
public class SerieDto
{
    /// <summary>
    /// Unique identifier for the media.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Title of the serie.
    /// </summary>
    public string Title { get; set; } = null!;
    
    /// <summary>
    /// Release date of the serie.
    /// </summary>
    public DateOnly ReleaseDate { get; set; }
    
    /// <summary>
    /// Poster URL of the serie.
    /// </summary>
    public string? PosterUrl { get; set; }
    
    /// <summary>
    /// Current status of the serie.
    /// </summary>
    public string Status { get; set; } = null!;
    
    /// <summary>
    /// Rating given by the user to the serie.
    /// </summary>
    public double? Rating { get; set; }
}