using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Miru.Domain.Repositories;
using Miru.Shared.Common;

namespace Miru.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly MiruDbContext Context;
    protected readonly DbSet<T> DbSet;

    protected Repository(MiruDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    /// <inheritdoc cref="IRepository{T}.GetByIdAsync"/>
    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    /// <inheritdoc cref="IRepository{T}.GetItemsAsync"/>
    public virtual async Task<PagingResult<T>> GetItemsAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IQueryable<T>>? orderBy = null,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default,
        params string[] includeProperties)
    {
        IQueryable<T> query = DbSet;

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

        var totalCount = await query.LongCountAsync(cancellationToken);

        if (orderBy is not null)
        {
            query = orderBy(query);
        }

        var items = await query
            .Skip(pageIndex * countPerPage)
            .Take(countPerPage)
            .ToListAsync(cancellationToken);

        return new PagingResult<T>
        {
            TotalCount = totalCount,
            PageIndex = pageIndex,
            CountPerPage = countPerPage,
            Items = items
        };
    }
    
    /// <inheritdoc cref="IRepository{T}.AddAsync"/>
    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }
    
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public virtual void Update(T entity)
    {
        DbSet.Update(entity);
    }

    /// <inheritdoc cref="IRepository{T}.Delete"/>
    public void Delete(T entity)
    {
        DbSet.Remove(entity);
    }
}