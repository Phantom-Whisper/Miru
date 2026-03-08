using Microsoft.EntityFrameworkCore;
using Miru.Domain.Entities;
using Miru.Domain.Repositories;
using Miru.Shared.Common;

namespace Miru.Infrastructure.Persistence.Repositories;

public class EpisodeRepository(MiruDbContext context) : Repository<Episode>(context), IEpisodeRepository
{
    private static readonly Dictionary<EpisodeOrderingCriteria, Func<IQueryable<Episode>, IQueryable<Episode>>> OrderingFactory
        = new()
        {
            [EpisodeOrderingCriteria.None] = query => query,
            [EpisodeOrderingCriteria.ByEpisodeNumber] = query => query.OrderBy(e => e.EpisodeNumber),
            [EpisodeOrderingCriteria.ByEpisodeNumberDescending] = query => query.OrderByDescending(e => e.EpisodeNumber),
            [EpisodeOrderingCriteria.ByTitle] = query => query.OrderBy(e => e.Title),
            [EpisodeOrderingCriteria.ByTitleDescending] = query => query.OrderByDescending(e => e.Title),
            [EpisodeOrderingCriteria.ByRating] = query => query.OrderBy(e => e.Rating ?? 0),
            [EpisodeOrderingCriteria.ByRatingDescending] = query => query.OrderByDescending(e => e.Rating ?? 0)
        };

    /// <inheritdoc cref="IEpisodeRepository.GetBySeasonIdAsync"/> 
    public async Task<PagingResult<Episode>> GetBySeasonIdAsync(
        Guid seasonId,
        EpisodeOrderingCriteria orderingCriteria = EpisodeOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        return await GetItemsAsync(
            filter: e => e.SeasonId == seasonId,
            orderBy: OrderingFactory[orderingCriteria],
            pageIndex: pageIndex,
            countPerPage: countPerPage,
            cancellationToken: cancellationToken);
    }
    
    /// <inheritdoc cref="IEpisodeRepository.GetWatchedByUserAsync"/>
    public async Task<PagingResult<Episode>> GetWatchedByUserAsync(
        Guid userId,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        
        var query = DbSet
            .Include(e => e.Season)
                .ThenInclude(s => s.Serie)
            .Where(e => e.Season.Serie.UserId == userId && e.Watched)
            .OrderByDescending(e => e.WatchedAt);
        
        var totalCount = await query.LongCountAsync(cancellationToken);
        
        var items = await query
            .Skip(pageIndex * countPerPage)
            .Take(countPerPage)
            .ToListAsync(cancellationToken);
        
        return new PagingResult<Episode>
        {
            TotalCount = totalCount,
            PageIndex = pageIndex,
            CountPerPage = countPerPage,
            Items = items
        };
    }
    
    /// <inheritdoc cref="IEpisodeRepository.GetUnwatchedBySerieAsync"/>
    public async Task<PagingResult<Episode>> GetUnwatchedBySerieAsync(
        Guid userId,
        Guid serieId,
        EpisodeOrderingCriteria orderingCriteria = EpisodeOrderingCriteria.ByEpisodeNumber,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(e => e.Season)
                .ThenInclude(s => s.Serie)
            .Where(e => e.Season.Serie.UserId == userId 
                     && e.Season.SerieId == serieId 
                     && !e.Watched);
        
        query = OrderingFactory[orderingCriteria](query);
        
        var totalCount = await query.LongCountAsync(cancellationToken);
        
        var items = await query
            .Skip(pageIndex * countPerPage)
            .Take(countPerPage)
            .ToListAsync(cancellationToken);
        
        return new PagingResult<Episode>
        {
            TotalCount = totalCount,
            PageIndex = pageIndex,
            CountPerPage = countPerPage,
            Items = items
        };
    }
    
    /// <inheritdoc cref="IEpisodeRepository.GetNextUnwatchedEpisodeAsync"/>
    public async Task<Episode?> GetNextUnwatchedEpisodeAsync(
        Guid userId,
        Guid serieId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.Season)
                .ThenInclude(s => s.Serie)
            .Where(e => e.Season.Serie.UserId == userId 
                     && e.Season.SerieId == serieId 
                     && !e.Watched)
            .OrderBy(e => e.Season.SeasonNumber)
                .ThenBy(e => e.EpisodeNumber)
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    /// <inheritdoc cref="IEpisodeRepository.GetTopRatedByUserAsync"/>
    public async Task<PagingResult<Episode>> GetTopRatedByUserAsync(
        Guid userId,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(e => e.Season)
                .ThenInclude(s => s.Serie)
            .Where(e => e.Season.Serie.UserId == userId && e.Rating.HasValue)
            .OrderByDescending(e => e.Rating)
                .ThenBy(e => e.Title);
        
        var totalCount = await query.LongCountAsync(cancellationToken);
        
        var items = await query
            .Skip(pageIndex * countPerPage)
            .Take(countPerPage)
            .ToListAsync(cancellationToken);
        
        return new PagingResult<Episode>
        {
            TotalCount = totalCount,
            PageIndex = pageIndex,
            CountPerPage = countPerPage,
            Items = items
        };
    }
    
    /// <inheritdoc cref="IEpisodeRepository.GetRecentlyWatchedAsync"/>
    public async Task<PagingResult<Episode>> GetRecentlyWatchedAsync(
        Guid userId,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(e => e.Season)
                .ThenInclude(s => s.Serie)
            .Where(e => e.Season.Serie.UserId == userId 
                     && e.Watched 
                     && e.WatchedAt.HasValue)
            .OrderByDescending(e => e.WatchedAt);
        
        var totalCount = await query.LongCountAsync(cancellationToken);
        
        var items = await query
            .Skip(pageIndex * countPerPage)
            .Take(countPerPage)
            .ToListAsync(cancellationToken);
        
        return new PagingResult<Episode>
        {
            TotalCount = totalCount,
            PageIndex = pageIndex,
            CountPerPage = countPerPage,
            Items = items
        };
    }
    
    /// <inheritdoc cref="IEpisodeRepository.CountWatchedBySerieAsync"/>
    public async Task<int> CountWatchedBySerieAsync(
        Guid userId,
        Guid serieId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.Season)
            .Where(e => e.Season.Serie.UserId == userId 
                     && e.Season.SerieId == serieId 
                     && e.Watched)
            .CountAsync(cancellationToken);
    }
    
    /// <inheritdoc cref="IEpisodeRepository.CountBySerieAsync"/>
    public async Task<int> CountBySerieAsync(
        Guid userId,
        Guid serieId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.Season)
            .Where(e => e.Season.Serie.UserId == userId 
                     && e.Season.SerieId == serieId)
            .CountAsync(cancellationToken);
    }
}