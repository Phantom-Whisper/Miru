using Miru.Contracts.Repositories;

namespace Miru.Contracts.Persistence;

public interface IUnitOfWork : IDisposable
{
    IMovieRepository Movies { get; }
    ISerieRepository Series { get; }
    ISeasonRepository Seasons { get; }
    IEpisodeRepository Episodes { get; }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RejectChangesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}