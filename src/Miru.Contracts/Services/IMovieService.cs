using Miru.Contracts.Common;
using Miru.Contracts.DTOs.Movies;
using Miru.Domain;

namespace Miru.Contracts.Services;

public interface IMovieService
{
    Task<PagingResult<MovieDto>> GetMoviesAsync(
        MovieOrderingCriteria orderingCriteria = MovieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default);
    
    Task<MovieDto?> GetMovieByIdAsync(
        Guid movieId,
        CancellationToken cancellationToken = default);

    Task<PagingResult<MovieDto>> GetMoviesByStatusAsync(
        MediaStatus status,
        MovieOrderingCriteria orderingCriteria = MovieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default);

    Task<PagingResult<MovieDto>> SearchMoviesByTitleAsync(
        string title,
        MovieOrderingCriteria orderingCriteria = MovieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default);
    
    Task<MovieDetailsDto> CreateMovieAsync(
        CreateMovieDto createMovieDto,
        CancellationToken cancellationToken = default);
    
    Task<MovieDetailsDto> UpdateMovieAsync(
        Guid movieId,
        UpdateMovieDto updateMovieDto,
        CancellationToken cancellationToken = default);

    Task UpdateMovieStatusAsync(
        Guid movieId,
        MediaStatus status,
        CancellationToken cancellationToken = default);

    Task RateMovieAsync(
        Guid movieId,
        double rating,
        CancellationToken cancellationToken = default);

    Task DeleteMovieAsync(
        Guid movieId,
        CancellationToken cancellationToken = default);
}