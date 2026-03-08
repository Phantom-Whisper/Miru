using Miru.Domain.Entities;
using Miru.Shared.Common;

namespace Miru.Domain.Repositories;

public interface IEpisodeRepository : IRepository<Episode>
{
    /// <summary>
    /// Retrieves a paginated list of episodes belonging to a specific season.
    /// </summary>
    /// <param name="seasonId">
    /// The unique identifier of the season whose episodes should be retrieved.
    /// </param>
    /// <param name="orderingCriteria">
    /// The criteria used to order the episodes (for example by air date, number, or creation date).
    /// Defaults to <see cref="EpisodeOrderingCriteria.None"/>.
    /// </param>
    /// <param name="pageIndex">
    /// The zero-based index of the page to retrieve.
    /// Defaults to 0.
    /// </param>
    /// <param name="countPerPage">
    /// The number of episodes to include per page.
    /// Defaults to 10.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A <see cref="PagingResult{Episode}"/> containing the episodes for the specified season and paging information.
    /// </returns>
    Task<PagingResult<Episode>> GetBySeasonIdAsync(
        Guid seasonId,
        EpisodeOrderingCriteria orderingCriteria = EpisodeOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a paginated list of episodes watched by a specific user.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user whose watched episodes should be retrieved.
    /// </param>
    /// <param name="pageIndex">
    /// The zero-based index of the page to retrieve.
    /// Defaults to 0.
    /// </param>
    /// <param name="countPerPage">
    /// The number of episodes to include per page.
    /// Defaults to 10.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A <see cref="PagingResult{Episode}"/> containing the watched episodes and paging information.
    /// </returns>
    Task<PagingResult<Episode>> GetWatchedByUserAsync(
        Guid userId,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a paginated list of episodes from a specific series that the user has not yet watched.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user.
    /// </param>
    /// <param name="serieId">
    /// The unique identifier of the series.
    /// </param>
    /// <param name="orderingCriteria">
    /// The criteria used to order the episodes.
    /// Defaults to <see cref="EpisodeOrderingCriteria.ByEpisodeNumber"/>.
    /// </param>
    /// <param name="pageIndex">
    /// The zero-based index of the page to retrieve.
    /// Defaults to 0.
    /// </param>
    /// <param name="countPerPage">
    /// The number of episodes to include per page.
    /// Defaults to 10.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A <see cref="PagingResult{Episode}"/> containing the unwatched episodes and paging information.
    /// </returns>
    Task<PagingResult<Episode>> GetUnwatchedBySerieAsync(
        Guid userId,
        Guid serieId,
        EpisodeOrderingCriteria orderingCriteria = EpisodeOrderingCriteria.ByEpisodeNumber,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves the next episode in a series that the user has not yet watched.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user.
    /// </param>
    /// <param name="serieId">
    /// The unique identifier of the series.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// The next unwatched <see cref="Episode"/> if one exists; otherwise, <c>null</c>.
    /// </returns>
    Task<Episode?> GetNextUnwatchedEpisodeAsync(
        Guid userId,
        Guid serieId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a paginated list of the highest-rated episodes rated by a specific user.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user.
    /// </param>
    /// <param name="pageIndex">
    /// The zero-based index of the page to retrieve.
    /// Defaults to 0.
    /// </param>
    /// <param name="countPerPage">
    /// The number of episodes to include per page.
    /// Defaults to 10.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A <see cref="PagingResult{Episode}"/> containing the top-rated episodes and paging information.
    /// </returns>
    Task<PagingResult<Episode>> GetTopRatedByUserAsync(
        Guid userId,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a paginated list of episodes most recently watched by a specific user.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user.
    /// </param>
    /// <param name="pageIndex">
    /// The zero-based index of the page to retrieve.
    /// Defaults to 0.
    /// </param>
    /// <param name="countPerPage">
    /// The number of episodes to include per page.
    /// Defaults to 10.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A <see cref="PagingResult{Episode}"/> containing the recently watched episodes and paging information.
    /// </returns>
    Task<PagingResult<Episode>> GetRecentlyWatchedAsync(
        Guid userId,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts the number of episodes from a specific series that the user has watched.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user.
    /// </param>
    /// <param name="serieId">
    /// The unique identifier of the series.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// The number of watched episodes.
    /// </returns>
    Task<int> CountWatchedBySerieAsync(
        Guid userId,
        Guid serieId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts the total number of episodes from a specific series available to the user.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user.
    /// </param>
    /// <param name="serieId">
    /// The unique identifier of the series.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// The total number of episodes.
    /// </returns>
    Task<int> CountBySerieAsync(
        Guid userId,
        Guid serieId,
        CancellationToken cancellationToken = default);
}