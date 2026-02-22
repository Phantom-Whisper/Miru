using Miru.Contracts.DTOs.Episodes;

namespace Miru.Contracts.DTOs.Seasons;

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