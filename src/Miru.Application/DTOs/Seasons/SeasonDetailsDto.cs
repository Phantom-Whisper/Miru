using Miru.Application.DTOs.Episodes;

namespace Miru.Application.DTOs.Seasons;

/// <summary>
/// Complete season information including episodes.
/// </summary>
public class SeasonDetailsDto : SeasonDto
{
    /// <summary>
    /// Lists of episodes in the season.
    /// </summary>
    public List<EpisodeDto> Episodes { get; set; } = [];
}