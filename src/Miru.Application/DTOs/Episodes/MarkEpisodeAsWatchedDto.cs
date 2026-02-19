namespace Miru.Application.DTOs.Episodes;

/// <summary>
/// DTO for marking an episode as watched.
/// </summary>
public class MarkEpisodeAsWatchedDto
{
    /// <summary>
    /// Custom watched date (optional, defaults to now).
    /// </summary>
    public DateTime? WatchedAt { get; set; }
}