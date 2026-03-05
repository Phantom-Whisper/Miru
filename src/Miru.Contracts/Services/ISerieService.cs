using Miru.Contracts.Common;
using Miru.Contracts.DTOs.Series;

namespace Miru.Contracts.Services;

public interface ISerieService
{
    Task<PagingResult<SerieDto>> GetSeriesAsync(
        SerieOrderingCriteria orderingCriteria = SerieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default);
    
    Task<SerieDetailsDto?> GetSerieByIdAsync(
        Guid serieId,
        CancellationToken cancellationToken = default);
    
    Task<PagingResult<SerieDto>> GetSeriesByStatusAsync(
        string status,
        SerieOrderingCriteria orderingCriteria = SerieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default);
    
    Task<SerieDetailsDto> CreateSerieAsync(
        CreateSerieDto createSerieDto,
        CancellationToken cancellationToken = default);
    
    Task<SerieDetailsDto> UpdateSerieAsync(
        Guid serieId,
        UpdateSerieDto updateSerieDto,
        CancellationToken cancellationToken = default);
    
    Task UpdateSerieStatusAsync(
        Guid serieId,
        string status,
        CancellationToken cancellationToken = default);
    
    Task RateSerieAsync(
        Guid serieId,
        double rating,
        CancellationToken cancellationToken = default);
    
    Task DeleteSerieAsync(
        Guid serieId,
        CancellationToken cancellationToken = default);
}