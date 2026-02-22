using Miru.Contracts.Common;
using Miru.Contracts.DTOs.Series;

namespace Miru.Contracts.Services;

public interface ISerieService
{
    Task<PagingResult<SerieDto>> GetSeriesAsync(
        Guid userId,
        SerieOrderingCriteria orderingCriteria = SerieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default);
    
    Task<SerieDetailsDto?> GetSerieByIdAsync(
        Guid userId,
        Guid serieId,
        CancellationToken cancellationToken = default);
    
    Task<PagingResult<SerieDto>> GetSeriesByStatusAsync(
        Guid userId,
        string status,
        SerieOrderingCriteria orderingCriteria = SerieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default);
    
    Task<SerieDetailsDto> CreateSerieAsync(
        Guid userId,
        CreateSerieDto createSerieDto,
        CancellationToken cancellationToken = default);
    
    Task<SerieDetailsDto> UpdateSerieAsync(
        Guid userId,
        Guid serieId,
        UpdateSerieDto updateSerieDto,
        CancellationToken cancellationToken = default);
    
    Task UpdateSerieStatusAsync(
        Guid userId,
        Guid serieId,
        string status,
        CancellationToken cancellationToken = default);
    
    Task RateSerieAsync(
        Guid userId,
        Guid serieId,
        double rating,
        CancellationToken cancellationToken = default);
    
    Task DeleteSerieAsync(
        Guid userId,
        Guid serieId,
        CancellationToken cancellationToken = default);
}