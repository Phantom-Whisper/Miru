using System.ComponentModel.DataAnnotations;

namespace Miru.Application.DTOs.Movies;

/// <summary>
/// DTO for updating movie status.
/// </summary>
public class UpdateMovieStatusDto
{
    /// <summary>
    /// New status for the movie (ToWatch, Watching, Watched).
    /// </summary>
    [Required(ErrorMessage = "The field {0} is required")]
    [RegularExpression("^(ToWatch|Watching|Watched)$",  ErrorMessage = "Invalid status. Must be ToWatch, Watching, or Watched")]
    public string Status { get; set; } = null!;
}