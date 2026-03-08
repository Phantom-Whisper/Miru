using System.ComponentModel.DataAnnotations;

namespace Miru.Shared.DTOs.Movies;

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
    public double Rating { get; set; }
}