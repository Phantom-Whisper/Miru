using Miru.Domain.Entities;
using Miru.Shared.DTOs.Seasons;

namespace Miru.Application.Mappings;

public static class SeasonMapping
{
    public static SeasonDto ToDto(this Season src) => new()
    {
        Id                   = src.Id,
        SeasonNumber         = src.SeasonNumber,
        ReleaseDate          = src.ReleaseDate,
        EpisodesCount        = src.Episodes.Count,
        WatchedEpisodesCount = src.Episodes.Count(e => e.Watched),
        ProgressPercentage   = src.CalculateProgress(),
    };

    public static SeasonDetailsDto ToDetailsDto(this Season? src) => new()
    {
        Id                   = src.Id,
        SeasonNumber         = src.SeasonNumber,
        ReleaseDate          = src.ReleaseDate,
        EpisodesCount        = src.Episodes.Count,
        WatchedEpisodesCount = src.Episodes.Count(e => e.Watched),
        ProgressPercentage   = src.CalculateProgress(),
        Episodes             = src.Episodes.Select(e => e.ToDto()).ToList(),
    };

    public static Season ToEntity(this CreateSeasonDto src, Guid serieId) =>
        Season.Create(
            seasonNumber: src.SeasonNumber,
            releaseDate:  src.ReleaseDate,
            serieId:      serieId
        );

    public static void ApplyUpdate(this UpdateSeasonDto src, Season season)
    {
        if (src.SeasonNumber is not null) season.UpdateSeasonNumber(src.SeasonNumber.Value);
        if (src.ReleaseDate is not null)  season.UpdateReleaseDate(src.ReleaseDate.Value);
    }

    private static double CalculateProgress(this Season src)
    {
        if (src.Episodes.Count == 0) return 0;
        var watched = src.Episodes.Count(e => e.Watched);
        return Math.Round((double)watched / src.Episodes.Count * 100, 2);
    }
}