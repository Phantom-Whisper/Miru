using Miru.Domain;

namespace Miru.Contracts.DTOs.Series;

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
    public MediaStatus Status { get; set; }
    
    /// <summary>
    /// Rating given by the user to the serie.
    /// </summary>
    public double? Rating { get; set; }
    
    /// <summary>
    /// Total number of seasons.
    /// </summary>
    public int SeasonsCount { get; set; }
    
    /// <summary>
    /// Total number of episodes across all seasons.
    /// </summary>
    public int TotalEpisodesCount { get; set; }
    
    /// <summary>
    /// Number of watched episodes.
    /// </summary>
    public int WatchedEpisodesCount { get; set; }
    
    /// <summary>
    /// Viewing progress percentage (0-100).
    /// </summary>
    public double ProgressPercentage { get; set; }
}