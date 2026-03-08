using System.ComponentModel.DataAnnotations;

namespace Miru.Shared.DTOs.Seasons;

/// <summary>
/// DTO for creating a new season.
/// </summary>
public class CreateSeasonDto
{
    /// <summary>
    /// Season number (must be unique within the serie).
    /// </summary>
    [Required(ErrorMessage = "The field {0} is required")]
    [Range(1, 100, ErrorMessage = "The field {0} must be between {1} and {2}")]
    public int SeasonNumber { get; set; }
    
    /// <summary>
    /// Release date of the season.
    /// </summary>
    [Required(ErrorMessage = "The field {0} is required")]
    public DateOnly ReleaseDate { get; set; }
}