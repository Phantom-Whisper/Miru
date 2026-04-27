using Miru.Domain.Entities;
using Miru.Shared.DTOs.Series;

namespace Miru.Application.Mappings;

public static class SerieMapping
{
    public static SerieDto ToDto(this Serie src) => new()
    {
        Id                   = src.Id,
        Title                = src.Title,
        ReleaseDate          = src.ReleaseDate,
        PosterUrl            = src.PosterUrl,
        Status               = src.Status,
        Rating               = src.Rating,
        SeasonsCount         = src.Seasons.Count,
        TotalEpisodesCount   = src.Seasons.Sum(s => s.Episodes.Count),
        WatchedEpisodesCount = src.Seasons.Sum(s => s.Episodes.Count(e => e.Watched)),
        ProgressPercentage   = src.CalculateProgress(),
    };

    public static SerieDetailsDto ToDetailsDto(this Serie src) => new()
    {
        Id                   = src.Id,
        Title                = src.Title,
        ReleaseDate          = src.ReleaseDate,
        PosterUrl            = src.PosterUrl,
        Status               = src.Status,
        Rating               = src.Rating,
        SeasonsCount         = src.Seasons.Count,
        TotalEpisodesCount   = src.Seasons.Sum(s => s.Episodes.Count),
        WatchedEpisodesCount = src.Seasons.Sum(s => s.Episodes.Count(e => e.Watched)),
        ProgressPercentage   = src.CalculateProgress(),
        Description          = src.Description,
        AddedAt              = src.AddedAt,
        WatchedAt            = src.WatchedAt,
        Seasons              = src.Seasons.Select(s => s.ToDto()).ToList(),
    };

    public static Serie ToEntity(this CreateSerieDto src, Guid userId) =>
        Serie.Create(
            userId:      userId,
            title:       src.Title,
            releaseDate: src.ReleaseDate,
            description: src.Description,
            posterUrl:   src.PosterUrl
        );

    public static void ApplyUpdate(this UpdateSerieDto src, Serie serie)
    {
        if (src.Title is not null)       serie.UpdateTitle(src.Title);
        if (src.ReleaseDate is not null) serie.UpdateReleaseDate(src.ReleaseDate.Value);
        if (src.Description is not null) serie.UpdateDescription(src.Description);
        if (src.PosterUrl is not null)   serie.UpdatePosterUrl(src.PosterUrl);
    }

    // Private helpers
    private static double CalculateProgress(this Serie src)
    {
        var total = src.Seasons.Sum(s => s.Episodes.Count);
        if (total == 0) return 0;
        var watched = src.Seasons.Sum(s => s.Episodes.Count(e => e.Watched));
        return Math.Round((double)watched / total * 100, 2);
    }
}