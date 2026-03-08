namespace Miru.Shared.DTOs.Episodes;

/// <summary>
/// Lightweight movie information for lists.
/// </summary>
public class EpisodeDto
{
    /// <summary>
    /// Unique identifier of the episode.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Episode number within the season.
    /// </summary>
    public int EpisodeNumber { get; set; }

    /// <summary>
    /// Episode title.
    /// </summary>
    public string Title { get; set; } = null!;
    
    /// <summary>
    /// Duration of the episode in minutes.
    /// </summary>
    public int Duration { get; set; }
    
    /// <summary>
    /// Tells if the episode's watched.
    /// </summary>
    public bool Watched { get; set; }
    
    /// <summary>
    /// Date the episode was watched by the user.
    /// </summary>
    public DateTime? WatchedAt { get; set; }
    
    /// <summary>
    /// Rating given by the user to the episode.
    /// </summary>
    public double? Rating { get; set; }
}