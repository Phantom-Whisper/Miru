using System.Linq.Expressions;
using Miru.Shared.Common;

namespace Miru.Domain.Repositories;

public interface IRepository<T> where T : class
{
    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the entity to retrieve.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the entity if found; otherwise, <c>null</c>.
    /// </returns>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of entities matching the specified criteria.
    /// </summary>
    /// <param name="filter">
    /// An optional filter expression to restrict the returned entities.
    /// If <c>null</c>, all entities are included.
    /// </param>
    /// <param name="orderBy">
    /// An optional function to order the query.
    /// </param>
    /// <param name="pageIndex">
    /// The zero-based index of the page to retrieve.
    /// Defaults to 0.
    /// </param>
    /// <param name="countPerPage">
    /// The number of entities to include per page.
    /// Defaults to 10.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <param name="includeProperties">
    /// Navigation properties to include in the query results.
    /// </param>
    /// <returns>
    /// A <see cref="PagingResult{T}"/> containing the entities and paging information.
    /// </returns>
    Task<PagingResult<T>> GetItemsAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IQueryable<T>>? orderBy = null,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default,
        params string[] includeProperties);
    
    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">
    /// The entity to add.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous add operation.
    /// </returns>
    Task AddAsync(
        T entity, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">
    /// The entity to update.
    /// </param>
    void Update(T entity);
    
    /// <summary>
    /// Removes an entity from the repository.
    /// </summary>
    /// <param name="entity">
    /// The entity to delete.
    /// </param>
    void Delete(T entity);
}