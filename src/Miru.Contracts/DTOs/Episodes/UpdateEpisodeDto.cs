using System.ComponentModel.DataAnnotations;

namespace Miru.Contracts.DTOs.Episodes;

/// <summary>
/// DTO for updating episode information.
/// </summary>
public class UpdateEpisodeDto
{
    /// <summary>
    /// Episode number within the season.
    /// </summary>
    [Range(1, 1000, ErrorMessage = "The field {0} must be between {1} and {2}")]
    public int? EpisodeNumber { get; set; }
    
    /// <summary>
    /// Title of the episode.
    /// </summary>
    [MaxLength(256, ErrorMessage = "The field {0} cannot exceed {1} characters")]
    public string? Title { get; set; }
    
    /// <summary>
    /// Duration of the episode in minutes.
    /// </summary>
    [Range(1, 600, ErrorMessage = "The field {0} must be between {1} and {2} minutes")]
    public int? DurationMinutes { get; set; }
}