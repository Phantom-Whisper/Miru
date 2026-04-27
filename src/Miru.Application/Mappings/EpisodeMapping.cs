using Miru.Domain.Entities;
using Miru.Shared.DTOs.Episodes;

namespace Miru.Application.Mappings;

public static class EpisodeMapping
{
    public static EpisodeDto ToDto(this Episode src) => new()
    {
        Id            = src.Id,
        EpisodeNumber = src.EpisodeNumber,
        Title         = src.Title,
        Duration      = (int)src.Duration.TotalMinutes,
        Watched       = src.Watched,
        WatchedAt     = src.WatchedAt,
        Rating        = src.Rating,
    };

    public static Episode ToEntity(this CreateEpisodeDto src, Guid seasonId) =>
        Episode.Create(
            episodeNumber: src.EpisodeNumber,
            title:         src.Title,
            duration:      TimeSpan.FromMinutes(src.DurationMinutes),
            seasonId:      seasonId
        );

    public static void ApplyUpdate(this UpdateEpisodeDto src, Episode episode)
    {
        if (src.EpisodeNumber is not null) episode.UpdateEpisodeNumber(src.EpisodeNumber.Value);
        if (src.Title is not null)         episode.UpdateTitle(src.Title);
        if (src.DurationMinutes is not null) episode.UpdateDuration(TimeSpan.FromMinutes(src.DurationMinutes.Value));
    }
}