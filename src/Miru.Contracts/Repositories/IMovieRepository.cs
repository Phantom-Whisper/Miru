using Miru.Contracts.Common;
using Miru.Domain;

namespace Miru.Contracts.Repositories;

public interface IMovieRepository : IRepository<Movie>
{
    /// <summary>
    /// Retrieves a paginated list of movies associated with a specific user.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user whose movies should be retrieved.
    /// </param>
    /// <param name="orderingCriteria">
    /// The criteria used to order the movies (for example by title, release date, or date added).
    /// Defaults to <see cref="MovieOrderingCriteria.None"/>.
    /// </param>
    /// <param name="pageIndex">
    /// The zero-based index of the page to retrieve.
    /// Defaults to 0.
    /// </param>
    /// <param name="countPerPage">
    /// The number of movies to include per page.
    /// Defaults to 10.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A <see cref="PagingResult{Movie}"/> containing the movies and paging information.
    /// </returns>
    Task<PagingResult<Movie>> GetMoviesAsync(
        Guid userId,
        MovieOrderingCriteria orderingCriteria = MovieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a paginated list of movies associated with a specific user filtered by their status.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user whose movies should be retrieved.
    /// </param>
    /// <param name="status">
    /// The status used to filter the movies (for example Watching, Completed, Planned, or Dropped).
    /// </param>
    /// <param name="orderingCriteria">
    /// The criteria used to order the movies.
    /// Defaults to <see cref="MovieOrderingCriteria.None"/>.
    /// </param>
    /// <param name="pageIndex">
    /// The zero-based index of the page to retrieve.
    /// Defaults to 0.
    /// </param>
    /// <param name="countPerPage">
    /// The number of movies to include per page.
    /// Defaults to 10.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A <see cref="PagingResult{Movie}"/> containing the filtered movies and paging information.
    /// </returns>
    Task<PagingResult<Movie>> GetMoviesByStatusAsync(
        Guid userId,
        MediaStatus status,
        MovieOrderingCriteria orderingCriteria = MovieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a paginated list of movies associated with a specific user that match the specified title.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user whose movies should be retrieved.
    /// </param>
    /// <param name="title">
    /// The title or partial title used to filter the movies.
    /// </param>
    /// <param name="orderingCriteria">
    /// The criteria used to order the movies.
    /// Defaults to <see cref="MovieOrderingCriteria.None"/>.
    /// </param>
    /// <param name="pageIndex">
    /// The zero-based index of the page to retrieve.
    /// Defaults to 0.
    /// </param>
    /// <param name="countPerPage">
    /// The number of movies to include per page.
    /// Defaults to 10.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A <see cref="PagingResult{Movie}"/> containing the matching movies and paging information.
    /// </returns>
    Task<PagingResult<Movie>> GetMoviesByTitleAsync(
        Guid userId,
        string title,
        MovieOrderingCriteria orderingCriteria = MovieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default);
}