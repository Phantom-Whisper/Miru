using Miru.Shared.DTOs.Seasons;

namespace Miru.Shared.DTOs.Series;

/// <summary>
/// Complete serie information including seasons.
/// </summary>
public class SerieDetailsDto : SerieDto
{
    /// <summary>
    /// Description of the serie.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Date the serie was added to the user's library.
    /// </summary>
    public DateTime AddedAt { get; set; }
    
    /// <summary>
    /// Date the serie was marked as watched.
    /// </summary>
    public DateTime? WatchedAt { get; set; }
    
    /// <summary>
    /// Lists of seasons in the serie.
    /// </summary>
    public List<SeasonDto> Seasons { get; set; } = [];
}