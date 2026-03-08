namespace Miru.Shared.DTOs.Seasons;

/// <summary>
/// Lightweight season information for lists.
/// </summary>
public class SeasonDto
{
    /// <summary>
    /// Unique identifier for the season.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Season number within the serie.
    /// </summary>
    public int SeasonNumber { get; set; }
    
    /// <summary>
    /// Date when the season was released.
    /// </summary>
    public DateOnly ReleaseDate { get; set; }
    
    /// <summary>
    /// Total number of episodes in this season.
    /// </summary>
    public int EpisodesCount { get; set; }
    
    /// <summary>
    /// Number of watched episodes in this season.
    /// </summary>
    public int WatchedEpisodesCount { get; set; }
    
    /// <summary>
    /// Viewing progress percentage for this season (0-100).
    /// </summary>
    public double ProgressPercentage { get; set; }
}