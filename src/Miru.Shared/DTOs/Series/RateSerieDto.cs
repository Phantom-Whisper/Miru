using System.ComponentModel.DataAnnotations;

namespace Miru.Shared.DTOs.Series;

/// <summary>
/// DTO for rating a serie.
/// </summary>
public class RateSerieDto
{
    /// <summary>
    /// Rating from 0 to 10 (one decimal place).
    /// </summary>
    [Required(ErrorMessage = "Rating is required")]
    [Range(0.0, 10.0, ErrorMessage = "Rating must be between 0 and 10")]
    public double Rating { get; set; }
}