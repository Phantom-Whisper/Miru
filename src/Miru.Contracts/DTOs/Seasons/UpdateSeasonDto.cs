using System.ComponentModel.DataAnnotations;

namespace Miru.Contracts.DTOs.Seasons;

/// <summary>
/// DTO for updating season information.
/// </summary>
public class UpdateSeasonDto
{
    /// <summary>
    /// Season number.
    /// </summary>
    [Range(1, 100, ErrorMessage = "The field {0} must be between {1} and {2}")]
    public int? SeasonNumber { get; set; }
    
    /// <summary>
    /// Release date of the season.
    /// </summary>
    public DateOnly? ReleaseDate { get; set; }
}