using Miru.Contracts.DTOs.Seasons;

namespace Miru.Contracts.Services;

public interface ISeasonService
{
    Task<IEnumerable<SeasonDto>> GetSeasonsBySerieIdAsync(
        Guid userId,
        Guid serieId,
        CancellationToken cancellationToken = default);
    
    Task<SeasonDetailsDto?> GetSeasonByIdAsync(
        Guid userId,
        Guid serieId,
        Guid seasonId,
        CancellationToken cancellationToken = default);
    
    Task<SeasonDetailsDto> AddSeasonToSerieAsync(
        Guid userId,
        Guid serieId,
        CreateSeasonDto createSeasonDto,
        CancellationToken cancellationToken = default);
    
    Task<SeasonDetailsDto> UpdateSeasonAsync(
        Guid userId,
        Guid serieId,
        Guid seasonId,
        UpdateSeasonDto updateSeasonDto,
        CancellationToken cancellationToken = default);
    
    Task DeleteSeasonAsync(
        Guid userId,
        Guid serieId,
        Guid seasonId,
        CancellationToken cancellationToken = default);
    
    Task MarkSeasonAsWatchedAsync(
        Guid userId,
        Guid serieId,
        Guid seasonId,
        CancellationToken cancellationToken = default);
}