using Miru.Contracts.Common;
using Miru.Domain;
using Miru.Domain.Entities;

namespace Miru.Contracts.Repositories;

public interface ISerieRepository : IRepository<Serie>
{
    /// <summary>
    /// Retrieves a paginated list of series associated with a specific user.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user whose series should be retrieved.
    /// </param>
    /// <param name="orderingCriteria">
    /// The criteria used to order the series.
    /// Defaults to <see cref="SerieOrderingCriteria.None"/>.
    /// </param>
    /// <param name="pageIndex">
    /// The zero-based index of the page to retrieve.
    /// Defaults to 0.
    /// </param>
    /// <param name="countPerPage">
    /// The number of series to include per page.
    /// Defaults to 10.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A <see cref="PagingResult{Serie}"/> containing the series and paging information.
    /// </returns>
    Task<PagingResult<Serie>> GetSeriesAsync(
        Guid userId,
        SerieOrderingCriteria orderingCriteria = SerieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a paginated list of series associated with a specific user filtered by status.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user whose series should be retrieved.
    /// </param>
    /// <param name="status">
    /// The status used to filter the series.
    /// </param>
    /// <param name="orderingCriteria">
    /// The criteria used to order the series.
    /// Defaults to <see cref="SerieOrderingCriteria.None"/>.
    /// </param>
    /// <param name="pageIndex">
    /// The zero-based index of the page to retrieve.
    /// Defaults to 0.
    /// </param>
    /// <param name="countPerPage">
    /// The number of series to include per page.
    /// Defaults to 10.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A <see cref="PagingResult{Serie}"/> containing the filtered series and paging information.
    /// </returns>
    Task<PagingResult<Serie>> GetSeriesByStatusAsync(
        Guid userId,
        MediaStatus status,
        SerieOrderingCriteria orderingCriteria = SerieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a paginated list of series associated with a specific user that match the specified title.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user whose series should be retrieved.
    /// </param>
    /// <param name="title">
    /// The title or partial title used to filter the series.
    /// </param>
    /// <param name="orderingCriteria">
    /// The criteria used to order the series.
    /// Defaults to <see cref="SerieOrderingCriteria.None"/>.
    /// </param>
    /// <param name="pageIndex">
    /// The zero-based index of the page to retrieve.
    /// Defaults to 0.
    /// </param>
    /// <param name="countPerPage">
    /// The number of series to include per page.
    /// Defaults to 10.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A <see cref="PagingResult{Serie}"/> containing the matching series and paging information.
    /// </returns>
    Task<PagingResult<Serie>> GetSeriesByTitleAsync(
        Guid userId,
        string title,
        SerieOrderingCriteria orderingCriteria = SerieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a series by its identifier, including its seasons.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the series.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// The series with its seasons if found; otherwise, <c>null</c>.
    /// </returns>
    Task<Serie?> GetByIdWithSeasonsAsync(
        Guid id, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a series by its identifier, including its seasons and their episodes.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the series.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// The series with its seasons and episodes if found; otherwise, <c>null</c>.
    /// </returns>
    Task<Serie?> GetByIdWithSeasonsAndEpisodesAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}