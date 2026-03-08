using Miru.Domain;
using Miru.Shared.Common;
using Miru.Shared.Common.Enums;
using Miru.Shared.DTOs.Series;

namespace Miru.Shared.Services;

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
        MediaStatus status,
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