using System.ComponentModel.DataAnnotations;

namespace Miru.Application.DTOs.Series;

/// <summary>
/// DTO for creating a new serie.
/// </summary>
public class CreateSerieDto
{
    /// <summary>
    /// Title of the serie.
    /// </summary>
    [Required(ErrorMessage = "The field {0} is required")]
    [MaxLength(256, ErrorMessage = "Title cannot exceed {0} characters")]
    public string Title { get; set; } = null!;
    
    /// <summary>
    /// Release date of the serie.
    /// </summary>
    [Required(ErrorMessage = "The field {0} is required")]
    public DateOnly ReleaseDate { get; set; }
    
    /// <summary>
    /// Description of the serie.
    /// </summary>
    [MaxLength(2000, ErrorMessage = "The field {0} cannot exceed {1} characters")]
    public string? Description { get; set; }
    
    /// <summary>
    /// Poster URL of the serie.
    /// </summary>
    [Url(ErrorMessage = "The field {0} is not a valid url")]
    [MaxLength(1000, ErrorMessage = "The field {0} cannot exceed {1} characters")]
    public string? PosterUrl { get; set; }
}