using System.ComponentModel.DataAnnotations;

namespace Miru.Application.DTOs.Movies;

/// <summary>
/// DTO for creating a new movie.
/// </summary>
public class CreateMovieDto
{
    /// <summary>
    /// Title of the movie.
    /// </summary>
    [Required(ErrorMessage = "The field {0} is required")]
    [MaxLength(256,  ErrorMessage = "The field {0} cannot exceed 256 characters")]
    public string Title { get; set; } = null!;
    
    /// <summary>
    /// Duration of the movie.
    /// </summary>
    [Required(ErrorMessage = "The field {0} is required")]
    [Range(1, 600, ErrorMessage = "The field {0} must be between {1} and {2} minutes")]
    public int Duration { get; set; }
    
    /// <summary>
    /// Release date of the movie.
    /// </summary>
    [Required(ErrorMessage = "The field {0} is required")]
    public DateOnly ReleaseDate { get; set; }
    
    /// <summary>
    /// Description of the movie.
    /// </summary>
    [MaxLength(2000, ErrorMessage = "The field {0} cannot exceed {1} characters")]
    public string? Description { get; set; }
    
    /// <summary>
    /// Poster URL of the movie.
    /// </summary>
    [Url(ErrorMessage = "The field {0} is not a valid url")]
    [MaxLength(1000, ErrorMessage = "The field {0} cannot exceed {1} characters")]
    public string? PosterUrl { get; set; }
}