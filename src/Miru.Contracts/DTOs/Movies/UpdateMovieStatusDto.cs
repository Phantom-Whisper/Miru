using System.ComponentModel.DataAnnotations;
using Miru.Domain;

namespace Miru.Contracts.DTOs.Movies;

/// <summary>
/// DTO for updating movie status.
/// </summary>
public class UpdateMovieStatusDto
{
    /// <summary>
    /// New status for the movie (ToWatch, Watching, Watched).
    /// </summary>
    [Required(ErrorMessage = "The field {0} is required")]
    public MediaStatus Status { get; set; }
}