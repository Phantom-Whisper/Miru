using Miru.Contracts.DTOs.Seasons;

namespace Miru.Contracts.Services;

public interface ISeasonService
{
    Task<IEnumerable<SeasonDto>> GetSeasonsBySerieIdAsync(
        Guid serieId,
        CancellationToken cancellationToken = default);
    
    Task<SeasonDetailsDto?> GetSeasonByIdAsync(
        Guid serieId,
        Guid seasonId,
        CancellationToken cancellationToken = default);
    
    Task<SeasonDetailsDto> AddSeasonToSerieAsync(
        Guid serieId,
        CreateSeasonDto createSeasonDto,
        CancellationToken cancellationToken = default);
    
    Task<SeasonDetailsDto> UpdateSeasonAsync(
        Guid serieId,
        Guid seasonId,
        UpdateSeasonDto updateSeasonDto,
        CancellationToken cancellationToken = default);
    
    Task DeleteSeasonAsync(
        Guid serieId,
        Guid seasonId,
        CancellationToken cancellationToken = default);
    
    Task MarkSeasonAsWatchedAsync(
        Guid serieId,
        Guid seasonId,
        CancellationToken cancellationToken = default);
}