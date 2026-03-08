using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Miru.Domain.Repositories;
using Miru.Infrastructure.Persistence.Repositories;

namespace Miru.Infrastructure.Persistence.UnitOfWork;

public class UnitOfWork(MiruDbContext context) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;
    
    private IMovieRepository? _movieRepository;
    private ISerieRepository? _serieRepository;
    private ISeasonRepository? _seasonRepository;
    private IEpisodeRepository? _episodeRepository;

    public IMovieRepository Movies => 
        _movieRepository ??= new MovieRepository(context);
    
    public ISerieRepository Series => 
        _serieRepository ??= new SerieRepository(context);
    
    public ISeasonRepository Seasons => 
        _seasonRepository ??= new SeasonRepository(context);
    
    public IEpisodeRepository Episodes => 
        _episodeRepository ??= new EpisodeRepository(context);
    
    /// <inheritdoc cref="IUnitOfWork.SaveChangesAsync"/>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await context.SaveChangesAsync(cancellationToken);
            
            foreach (var entry in context.ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Detached))
            {
                entry.State = EntityState.Detached;
            }
            
            return result;
        }
        catch (DbUpdateException)
        {
            await RejectChangesAsync(cancellationToken);
            throw;
        }
    }
    
    /// <inheritdoc cref="IUnitOfWork.RejectChangesAsync"/>
    public async Task RejectChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in context.ChangeTracker.Entries()
            .Where(e => e.State != EntityState.Unchanged))
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.State = EntityState.Detached;
                    break;
                    
                case EntityState.Modified:
                case EntityState.Deleted:
                    await entry.ReloadAsync(cancellationToken);
                    break;
            }
        }
    }
    
    /// <inheritdoc cref="IUnitOfWork.BeginTransactionAsync"/>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await context.Database.BeginTransactionAsync(cancellationToken);
    }
    
    /// <inheritdoc cref="IUnitOfWork.CommitTransactionAsync"/>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
            
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
    
    /// <inheritdoc cref="IUnitOfWork.RollbackTransactionAsync"/>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
    
    private bool _disposed;
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _transaction?.Dispose();
            context.Dispose();
        }
        _disposed = true;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}