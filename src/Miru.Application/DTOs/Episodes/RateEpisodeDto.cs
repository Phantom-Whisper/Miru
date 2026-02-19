using System.ComponentModel.DataAnnotations;

namespace Miru.Application.DTOs.Episodes;

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
    [RegularExpression(@"^\d{1,2}(\.\d{1})?$", ErrorMessage = "Rating must have at most one decimal place")]
    public double Rating { get; set; }
}