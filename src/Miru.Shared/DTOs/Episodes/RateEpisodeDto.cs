using System.ComponentModel.DataAnnotations;

namespace Miru.Shared.DTOs.Episodes;

/// <summary>
/// DTO for rating an episode.
/// </summary>
public class RateEpisodeDto
{
    /// <summary>
    /// Rating from 0 to 10 (one decimal place).
    /// </summary>
    [Required(ErrorMessage = "The field {0} is required")]
    [Range(0.0, 10.0, ErrorMessage = "The field {0} must be between {1} and {2}")]
    public double Rating { get; set; }
}