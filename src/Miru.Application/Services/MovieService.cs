using AutoMapper;
using Miru.Application.Exceptions;
using Miru.Contracts.Common;
using Miru.Contracts.DTOs.Movies;
using Miru.Contracts.Persistence;
using Miru.Contracts.Services;
using Miru.Domain;
using Miru.Domain.Entities;

namespace Miru.Application.Services;

public class MovieService : IMovieService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MovieService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<PagingResult<MovieDto>> GetMoviesAsync(Guid userId, MovieOrderingCriteria orderingCriteria = MovieOrderingCriteria.None, int pageIndex = 0,
        int countPerPage = 10, CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.Movies.GetMoviesAsync(
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
            Items = _mapper.Map<IEnumerable<MovieDto>>(result.Items)
        };
    }

    public async Task<MovieDto?> GetMovieByIdAsync(Guid userId, Guid movieId, CancellationToken cancellationToken = default)
    {
        var movie = await _unitOfWork.Movies.GetByIdAsync(movieId, cancellationToken);
        
        if (movie == null) return null;
        
        return movie.UserId != userId ? throw new ForbiddenException() : _mapper.Map<MovieDetailsDto>(movie);
    }

    public async Task<PagingResult<MovieDto>> GetMoviesByStatusAsync(Guid userId, string status,
        MovieOrderingCriteria orderingCriteria = MovieOrderingCriteria.None, int pageIndex = 0, int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<MediaStatus>(status, out var mediaStatus))
            throw new ValidationException($"Invalid status: {status}. Must be ToWatch, Watching, or Watched.");
        
        var result = await _unitOfWork.Movies.GetMoviesByStatusAsync(
            userId,
            mediaStatus,
            orderingCriteria,
            pageIndex,
            countPerPage,
            cancellationToken);

        return new PagingResult<MovieDto>
        {
            TotalCount = result.TotalCount,
            PageIndex = result.PageIndex,
            CountPerPage = result.CountPerPage,
            Items = _mapper.Map<IEnumerable<MovieDto>>(result.Items)
        };
    }

    public async Task<PagingResult<MovieDto>> SearchMoviesByTitleAsync(Guid userId, string title,
        MovieOrderingCriteria orderingCriteria = MovieOrderingCriteria.None, int pageIndex = 0, int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.Movies.GetMoviesByTitleAsync(
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
            Items = _mapper.Map<IEnumerable<MovieDto>>(result.Items)
        };
    }

    public async Task<MovieDetailsDto> CreateMovieAsync(Guid userId, CreateMovieDto createMovieDto, CancellationToken cancellationToken = default)
    {
        var movie = Movie.Create(
            userId,
            createMovieDto.Title,
            TimeSpan.FromMinutes(createMovieDto.Duration),
            createMovieDto.ReleaseDate,
            createMovieDto.Description,
            createMovieDto.PosterUrl);
        
        await _unitOfWork.Movies.AddAsync(movie, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return _mapper.Map<MovieDetailsDto>(movie);
    }

    public async Task<MovieDetailsDto> UpdateMovieAsync(Guid userId, Guid movieId, UpdateMovieDto updateMovieDto,
        CancellationToken cancellationToken = default)
    {
        var movie = await _unitOfWork.Movies.GetByIdAsync(movieId, cancellationToken);
        
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
        
        _unitOfWork.Movies.Update(movie);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return _mapper.Map<MovieDetailsDto>(movie);
    }

    public async Task UpdateMovieStatusAsync(Guid userId, Guid movieId, string status, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<MediaStatus>(status, out var mediaStatus))
            throw new ValidationException($"Invalid status: {status}");
        
        var movie = await _unitOfWork.Movies.GetByIdAsync(movieId, cancellationToken);
        
        if (movie == null)
            throw new NotFoundException("Movie", movieId);
        
        if (movie.UserId != userId)
            throw new ForbiddenException();
        
        movie.UpdateStatus(mediaStatus);
        
        _unitOfWork.Movies.Update(movie);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RateMovieAsync(Guid userId, Guid movieId, double rating, CancellationToken cancellationToken = default)
    {
        var movie = await _unitOfWork.Movies.GetByIdAsync(movieId, cancellationToken);
        
        if (movie == null)
            throw new NotFoundException("Movie", movieId);
        
        if (movie.UserId != userId)
            throw new ForbiddenException();
        
        movie.SetRating(rating);
        
        _unitOfWork.Movies.Update(movie);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteMovieAsync(Guid userId, Guid movieId, CancellationToken cancellationToken = default)
    {
        var movie = await _unitOfWork.Movies.GetByIdAsync(movieId, cancellationToken);
        
        if (movie == null)
            throw new NotFoundException("Movie", movieId);
        
        if (movie.UserId != userId)
            throw new ForbiddenException();
        
        _unitOfWork.Movies.Delete(movie);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}