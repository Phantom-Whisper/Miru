using System.ComponentModel.DataAnnotations;

namespace Miru.Application.DTOs.Movies;

/// <summary>
/// DTO for rating a movie.
/// </summary>
public class RateMovieDto
{
    /// <summary>
    /// Rating from 0 to 10.
    /// </summary>
    [Required(ErrorMessage = "The field {0} is required")]
    [Range(0.0, 10.0, ErrorMessage =  "The field {0} must be between {1} and {2}")]
    [RegularExpression(@"^\d{1,2}(\.\d{1})?$", ErrorMessage = "Rating must have at most one decimal place")]
    public double Rating { get; set; }
}