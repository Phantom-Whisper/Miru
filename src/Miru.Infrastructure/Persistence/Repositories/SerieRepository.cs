using Microsoft.EntityFrameworkCore;
using Miru.Domain.Entities;
using Miru.Domain.Repositories;
using Miru.Shared.Common;
using Miru.Shared.Common.Enums;

namespace Miru.Infrastructure.Persistence.Repositories;

public class SerieRepository(MiruDbContext context) : Repository<Serie>(context), ISerieRepository
{
    private static readonly Dictionary<SerieOrderingCriteria, Func<IQueryable<Serie>, IQueryable<Serie>>> OrderingFactory
        = new()
        {
            [SerieOrderingCriteria.None] = query => query,
            [SerieOrderingCriteria.ByTitle] = query => query.OrderBy(s => s.Title),
            [SerieOrderingCriteria.ByTitleDescending] = query => query.OrderByDescending(s => s.Title),
            [SerieOrderingCriteria.ByReleaseDate] = query => query.OrderBy(s => s.ReleaseDate),
            [SerieOrderingCriteria.ByReleaseDateDescending] = query => query.OrderByDescending(s => s.ReleaseDate),
            [SerieOrderingCriteria.ByRating] = query => query.OrderBy(s => s.Rating ?? 0),
            [SerieOrderingCriteria.ByRatingDescending] = query => query.OrderByDescending(s => s.Rating ?? 0),
            [SerieOrderingCriteria.ByAddedDate] = query => query.OrderBy(s => s.AddedAt),
            [SerieOrderingCriteria.ByAddedDateDescending] = query => query.OrderByDescending(s => s.AddedAt)
        };

    /// <inheritdoc cref="ISerieRepository.GetSeriesAsync"/>
    public async Task<PagingResult<Serie>> GetSeriesAsync(
        Guid userId,
        SerieOrderingCriteria orderingCriteria = SerieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        return await GetItemsAsync(
            filter: s => s.UserId == userId,
            orderBy: OrderingFactory[orderingCriteria],
            pageIndex: pageIndex,
            countPerPage: countPerPage,
            cancellationToken: cancellationToken,
            includeProperties: nameof(Serie.Seasons));
    }

    /// <inheritdoc cref="ISerieRepository.GetSeriesByStatusAsync"/>
    public async Task<PagingResult<Serie>> GetSeriesByStatusAsync(
        Guid userId,
        MediaStatus status,
        SerieOrderingCriteria orderingCriteria = SerieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        return await GetItemsAsync(
            filter: s => s.UserId == userId && s.Status == status,
            orderBy: OrderingFactory[orderingCriteria],
            pageIndex: pageIndex,
            countPerPage: countPerPage,
            cancellationToken: cancellationToken,
            includeProperties: nameof(Serie.Seasons));
    }
    
    /// <inheritdoc cref="ISerieRepository.GetSeriesByTitleAsync"/>
    public async Task<PagingResult<Serie>> GetSeriesByTitleAsync(
        Guid userId,
        string title,
        SerieOrderingCriteria orderingCriteria = SerieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        string loweredTitle = title.ToLower();
        
        return await GetItemsAsync(
            filter: s => s.UserId == userId && (s.Title).Contains(loweredTitle, StringComparison.CurrentCultureIgnoreCase),
            orderBy: OrderingFactory[orderingCriteria],
            pageIndex: pageIndex,
            countPerPage: countPerPage,
            cancellationToken: cancellationToken,
            includeProperties: nameof(Serie.Seasons));
    }
    
    /// <inheritdoc cref="ISerieRepository.GetByIdWithSeasonsAsync"/>
    public async Task<Serie?> GetByIdWithSeasonsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Seasons)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
    
    /// <inheritdoc cref="ISerieRepository.GetByIdWithSeasonsAndEpisodesAsync"/>
    public async Task<Serie?> GetByIdWithSeasonsAndEpisodesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Seasons)
                .ThenInclude(season => season.Episodes)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
}