using System.ComponentModel.DataAnnotations;
using Miru.Shared.Common.Enums;

namespace Miru.Shared.DTOs.Series;

/// <summary>
/// DTO for updating serie status.
/// </summary>
public class UpdateSerieStatusDto
{
    /// <summary>
    /// New status for the serie (ToWatch, Watching, Watched).
    /// </summary>
    [Required(ErrorMessage = "The field {0} is required")]
    public MediaStatus Status { get; set; }
}