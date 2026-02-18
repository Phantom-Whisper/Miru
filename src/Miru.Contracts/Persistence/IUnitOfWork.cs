using Miru.Contracts.Repositories;

namespace Miru.Contracts.Persistence;

public interface IUnitOfWork : IDisposable
{
    IMovieRepository Movies { get; }
    ISerieRepository Series { get; }
    ISeasonRepository Seasons { get; }
    IEpisodeRepository Episodes { get; }
    
    /// <summary>
    /// Persists all changes made in the current context to the database.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Reverts all uncommitted changes in the current context.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    Task RejectChangesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Begins a database transaction.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Commits the current database transaction.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Rolls back the current database transaction.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}