using Miru.Shared.Common;
using Miru.Shared.DTOs.Episodes;

namespace Miru.Shared.Services;

public interface IEpisodeService
{
    Task<PagingResult<EpisodeDto>> GetEpisodesBySeasonIdAsync(
        Guid serieId,
        Guid seasonId,
        EpisodeOrderingCriteria orderingCriteria = EpisodeOrderingCriteria.ByEpisodeNumber,
        int pageIndex = 0,
        int countPerPage = 50,
        CancellationToken cancellationToken = default);
    
    Task<EpisodeDto?> GetEpisodeByIdAsync(
        Guid serieId,
        Guid seasonId,
        Guid episodeId,
        CancellationToken cancellationToken = default);
    
    Task<EpisodeDto> AddEpisodeToSeasonAsync(
        Guid serieId,
        Guid seasonId,
        CreateEpisodeDto createEpisodeDto,
        CancellationToken cancellationToken = default);
    
    Task<EpisodeDto> UpdateEpisodeAsync(
        Guid serieId,
        Guid seasonId,
        Guid episodeId,
        UpdateEpisodeDto updateEpisodeDto,
        CancellationToken cancellationToken = default);
    
    Task MarkEpisodeAsWatchedAsync(
        Guid serieId,
        Guid seasonId,
        Guid episodeId,
        CancellationToken cancellationToken = default);
    
    Task MarkEpisodeAsUnwatchedAsync(
        Guid serieId,
        Guid seasonId,
        Guid episodeId,
        CancellationToken cancellationToken = default);
    
    Task RateEpisodeAsync(
        Guid serieId,
        Guid seasonId,
        Guid episodeId,
        double rating,
        CancellationToken cancellationToken = default);
    
    Task DeleteEpisodeAsync(
        Guid serieId,
        Guid seasonId,
        Guid episodeId,
        CancellationToken cancellationToken = default);
    
    Task<EpisodeDto?> GetNextUnwatchedEpisodeAsync(
        Guid serieId,
        CancellationToken cancellationToken = default);
    
    Task<PagingResult<EpisodeDto>> GetRecentlyWatchedEpisodesAsync(
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default);
}