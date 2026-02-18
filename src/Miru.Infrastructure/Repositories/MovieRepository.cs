using Microsoft.EntityFrameworkCore;
using Miru.Contracts.Common;
using Miru.Contracts.Repositories;
using Miru.Domain;
using Miru.Domain.Entities;

namespace Miru.Infrastructure.Repositories;

public class MovieRepository : Repository<Movie>, IMovieRepository
{
    private static readonly Dictionary<MovieOrderingCriteria, Func<IQueryable<Movie>, IQueryable<Movie>>> OrderingFactory
        = new()
        {
            [MovieOrderingCriteria.None] = query => query,
            [MovieOrderingCriteria.ByTitle] = query => query.OrderBy(m => m.Title),
            [MovieOrderingCriteria.ByTitleDescending] = query => query.OrderByDescending(m => m.Title),
            [MovieOrderingCriteria.ByReleaseDate] = query => query.OrderBy(m => m.ReleaseDate),
            [MovieOrderingCriteria.ByReleaseDateDescending] = query => query.OrderByDescending(m => m.ReleaseDate),
            [MovieOrderingCriteria.ByRating] = query => query.OrderBy(m => m.Rating ?? 0),
            [MovieOrderingCriteria.ByRatingDescending] = query => query.OrderByDescending(m => m.Rating ?? 0),
            [MovieOrderingCriteria.ByAddedDate] = query => query.OrderBy(m => m.AddedAt),
            [MovieOrderingCriteria.ByAddedDateDescending] = query => query.OrderByDescending(m => m.AddedAt)
        };

    public MovieRepository(MiruDbContext context)
        : base(context)
    {
    }

    /// <inheritdoc cref="IMovieRepository.GetMoviesAsync"/>
    public async Task<PagingResult<Movie>> GetMoviesAsync(
        Guid userId,
        MovieOrderingCriteria orderingCriteria = MovieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        return await GetItemsAsync(
            filter: m => m.UserId == userId,
            orderBy: OrderingFactory[orderingCriteria],
            pageIndex: pageIndex,
            countPerPage: countPerPage,
            cancellationToken: cancellationToken);
    }
    
    /// <inheritdoc cref="IMovieRepository.GetMoviesByStatusAsync"/>
    public async Task<PagingResult<Movie>> GetMoviesByStatusAsync(
        Guid userId,
        MediaStatus status,
        MovieOrderingCriteria orderingCriteria = MovieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        return await GetItemsAsync(
            filter: m => m.UserId == userId && m.Status == status,
            orderBy: OrderingFactory[orderingCriteria],
            pageIndex: pageIndex,
            countPerPage: countPerPage,
            cancellationToken: cancellationToken);
    }
    
    /// <inheritdoc cref="IMovieRepository.GetMoviesByTitleAsync"/>
    public async Task<PagingResult<Movie>> GetMoviesByTitleAsync(
        Guid userId,
        string title,
        MovieOrderingCriteria orderingCriteria = MovieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        return await GetItemsAsync(
            filter: m => m.UserId == userId &&
                         EF.Functions.Like(m.Title, $"%{title}%"),
            orderBy: OrderingFactory[orderingCriteria],
            pageIndex: pageIndex,
            countPerPage: countPerPage,
            cancellationToken: cancellationToken);
    }
}