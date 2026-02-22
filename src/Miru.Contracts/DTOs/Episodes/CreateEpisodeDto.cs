using System.ComponentModel.DataAnnotations;

namespace Miru.Contracts.DTOs.Episodes;

/// <summary>
/// DTO for creating a new episode.
/// </summary>
public class CreateEpisodeDto
{
    /// <summary>
    /// Episode number within the season.
    /// </summary>
    [Required(ErrorMessage = "The field {0} is required")]
    [Range(1, 1000, ErrorMessage = "The field {0} must be between {1} and {2}")]
    public int EpisodeNumber { get; set; }
    
    /// <summary>
    /// Title of the episode.
    /// </summary>
    [Required(ErrorMessage = "The field {0} is required")]
    [MaxLength(256, ErrorMessage = "The field {0} cannot exceed {1} characters")]
    public string Title { get; set; } = null!;
    
    /// <summary>
    /// Duration of the episode in minutes.
    /// </summary>
    [Required(ErrorMessage = "The field {0} is required")]
    [Range(1, 600, ErrorMessage = "The field {0} must be between {1} and {2} minutes")]
    public int DurationMinutes { get; set; }
}