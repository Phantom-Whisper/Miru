using Miru.Application.Exceptions;
using Miru.Application.Interfaces;
using Miru.Application.Mappings;
using Miru.Shared.Common;
using Miru.Shared.DTOs.Movies;
using Miru.Domain.Entities;
using Miru.Infrastructure.Persistence.UnitOfWork;
using Miru.Shared.Common.Enums;

namespace Miru.Application.Services;

public class MovieService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    : IMovieService
{
    public async Task<PagingResult<MovieDto>> GetMoviesAsync(MovieOrderingCriteria orderingCriteria = MovieOrderingCriteria.None, int pageIndex = 0,
        int countPerPage = 10, CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        
        var result = await unitOfWork.Movies.GetMoviesAsync(
            userId,
            orderingCriteria,
            pageIndex,
            countPerPage,
            cancellationToken);

        return new PagingResult<MovieDto>
        {
            TotalCount = result.TotalCount,
            PageIndex = result.PageIndex,
            CountPerPage = result.CountPerPage,
            Items = result.Items.Select(m => m.ToDto())
        };
    }

    public async Task<MovieDetailsDto?> GetMovieByIdAsync(Guid movieId, CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        
        var movie = await unitOfWork.Movies.GetByIdAsync(movieId, cancellationToken);
        
        if (movie == null) return null;
        
        return movie.UserId != userId ? throw new ForbiddenException() : movie.ToDetailsDto();
    }

    public async Task<PagingResult<MovieDto>> GetMoviesByStatusAsync(MediaStatus status,
        MovieOrderingCriteria orderingCriteria = MovieOrderingCriteria.None, int pageIndex = 0, int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        
        var result = await unitOfWork.Movies.GetMoviesByStatusAsync(
            userId,
            status,
            orderingCriteria,
            pageIndex,
            countPerPage,
            cancellationToken);

        return new PagingResult<MovieDto>
        {
            TotalCount = result.TotalCount,
            PageIndex = result.PageIndex,
            CountPerPage = result.CountPerPage,
            Items = result.Items.Select(m => m.ToDetailsDto())
        };
    }

    public async Task<PagingResult<MovieDto>> SearchMoviesByTitleAsync(string title,
        MovieOrderingCriteria orderingCriteria = MovieOrderingCriteria.None, int pageIndex = 0, int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        
        var result = await unitOfWork.Movies.GetMoviesByTitleAsync(
            userId,
            title,
            orderingCriteria,
            pageIndex,
            countPerPage,
            cancellationToken);

        return new PagingResult<MovieDto>
        {
            TotalCount = result.TotalCount,
            PageIndex = result.PageIndex,
            CountPerPage = result.CountPerPage,
            Items = result.Items.Select(m => m.ToDto())
        };
    }

    public async Task<MovieDetailsDto> CreateMovieAsync(CreateMovieDto createMovieDto, CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;

        var movie = Movie.Create(
            userId,
            createMovieDto.Title,
            TimeSpan.FromMinutes(createMovieDto.Duration),
            createMovieDto.ReleaseDate,
            createMovieDto.Description,
            createMovieDto.PosterUrl);
        
        await unitOfWork.Movies.AddAsync(movie, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return movie.ToDetailsDto();
    }

    public async Task<MovieDetailsDto> UpdateMovieAsync(Guid movieId, UpdateMovieDto updateMovieDto,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;

        var movie = await unitOfWork.Movies.GetByIdAsync(movieId, cancellationToken);
        
        if (movie is null)
            throw new NotFoundException("Movie", movieId);
        
        if (movie.UserId != userId)
            throw new ForbiddenException();
        
        if (!string.IsNullOrWhiteSpace(updateMovieDto.Title))
            movie.UpdateTitle(updateMovieDto.Title);
        
        if (updateMovieDto.Duration.HasValue)
            movie.UpdateDuration(TimeSpan.FromMinutes(updateMovieDto.Duration.Value));
        
        if (updateMovieDto.ReleaseDate.HasValue)
            movie.UpdateReleaseDate(updateMovieDto.ReleaseDate.Value);
        
        if (updateMovieDto.Description != null)
            movie.UpdateDescription(updateMovieDto.Description);
        
        if (updateMovieDto.PosterUrl != null)
            movie.UpdatePosterUrl(updateMovieDto.PosterUrl);
        
        unitOfWork.Movies.Update(movie);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return movie.ToDetailsDto();
    }

    public async Task UpdateMovieStatusAsync(Guid movieId, MediaStatus status, CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;

        
        var movie = await unitOfWork.Movies.GetByIdAsync(movieId, cancellationToken);
        
        if (movie == null)
            throw new NotFoundException("Movie", movieId);
        
        if (movie.UserId != userId)
            throw new ForbiddenException();
        
        movie.UpdateStatus(status);
        
        unitOfWork.Movies.Update(movie);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RateMovieAsync(Guid movieId, double rating, CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;

        var movie = await unitOfWork.Movies.GetByIdAsync(movieId, cancellationToken);
        
        if (movie == null)
            throw new NotFoundException("Movie", movieId);
        
        if (movie.UserId != userId)
            throw new ForbiddenException();
        
        movie.SetRating(rating);
        
        unitOfWork.Movies.Update(movie);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteMovieAsync(Guid movieId, CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;

        var movie = await unitOfWork.Movies.GetByIdAsync(movieId, cancellationToken);
        
        if (movie == null)
            throw new NotFoundException("Movie", movieId);
        
        if (movie.UserId != userId)
            throw new ForbiddenException();
        
        unitOfWork.Movies.Delete(movie);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}