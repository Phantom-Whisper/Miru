using System.ComponentModel.DataAnnotations;

namespace Miru.Application.DTOs.Series;

/// <summary>
/// DTO for updating serie status.
/// </summary>
public class UpdateSerieStatusDto
{
    /// <summary>
    /// New status for the serie (ToWatch, Watching, Watched).
    /// </summary>
    [Required(ErrorMessage = "The field {0} is required")]
    [RegularExpression("^(ToWatch|Watching|Watched)$", ErrorMessage = "Invalid status. Must be ToWatch, Watching, or Watched")]
    public string Status { get; set; } = null!;
}