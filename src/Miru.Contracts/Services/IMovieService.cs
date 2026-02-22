using Miru.Contracts.Common;
using Miru.Contracts.DTOs.Movies;

namespace Miru.Contracts.Services;

public interface IMovieService
{
    Task<PagingResult<MovieDto>> GetMoviesAsync(
        Guid userId,
        MovieOrderingCriteria orderingCriteria = MovieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default);
    
    Task<MovieDto?> GetMovieByIdAsync(
        Guid userId,
        Guid movieId,
        CancellationToken cancellationToken = default);

    Task<PagingResult<MovieDto>> GetMoviesByStatusAsync(
        Guid userId,
        string status,
        MovieOrderingCriteria orderingCriteria = MovieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default);

    Task<PagingResult<MovieDto>> SearchMoviesByTitleAsync(
        Guid userId,
        string title,
        MovieOrderingCriteria orderingCriteria = MovieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default);
    
    Task<MovieDetailsDto> CreateMovieAsync(
        Guid userId,
        CreateMovieDto createMovieDto,
        CancellationToken cancellationToken = default);
    
    Task<MovieDetailsDto> UpdateMovieAsync(
        Guid userId,
        Guid movieId,
        UpdateMovieDto updateMovieDto,
        CancellationToken cancellationToken = default);

    Task UpdateMovieStatusAsync(
        Guid userId,
        Guid movieId,
        string status,
        CancellationToken cancellationToken = default);

    Task RateMovieAsync(
        Guid userId,
        Guid movieId,
        double rating,
        CancellationToken cancellationToken = default);

    Task DeleteMovieAsync(
        Guid userId,
        Guid movieId,
        CancellationToken cancellationToken = default);
}