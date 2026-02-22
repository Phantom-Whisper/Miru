using AutoMapper;
using Miru.Application.Exceptions;
using Miru.Application.Services;
using Miru.Contracts.Common;
using Miru.Contracts.DTOs.Movies;
using Miru.Contracts.Persistence;
using Miru.Contracts.Repositories;
using Miru.Domain.Entities;
using Moq;

namespace Miru.Test.Application;

public class MovieServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMovieRepository> _mockMovieRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly MovieService _movieService;
    
    public MovieServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMovieRepository = new Mock<IMovieRepository>();
        _mockMapper = new Mock<IMapper>();
        
        _mockUnitOfWork.Setup(u => u.Movies).Returns(_mockMovieRepository.Object);
        
        _movieService = new MovieService(_mockUnitOfWork.Object, _mockMapper.Object);
    }
    
    [Fact]
    public async Task GetUserMoviesAsync_ReturnsMovies()
    {
        var userId = Guid.NewGuid();
        var movies = new List<Movie>
        {
            Movie.Create(userId, "Movie 1", TimeSpan.FromMinutes(120), new DateOnly(2024, 1, 1)),
            Movie.Create(userId, "Movie 2", TimeSpan.FromMinutes(130), new DateOnly(2024, 1, 2))
        };
        
        var pagingResult = new PagingResult<Movie>
        {
            TotalCount = 2,
            PageIndex = 0,
            CountPerPage = 10,
            Items = movies
        };
        
        var movieDtos = movies.Select(m => new MovieDto { Id = m.Id, Title = m.Title }).ToList();
        
        _mockMovieRepository
            .Setup(r => r.GetMoviesAsync(
                userId,
                MovieOrderingCriteria.None,
                0,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagingResult);
        
        _mockMapper
            .Setup(m => m.Map<IEnumerable<MovieDto>>(It.IsAny<IEnumerable<Movie>>()))
            .Returns(movieDtos);
        
        var result = await _movieService.GetMoviesAsync(userId);
        
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
        _mockMovieRepository.Verify(r => r.GetMoviesAsync(
            userId,
            MovieOrderingCriteria.None,
            0,
            10,
            It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task GetMovieByIdAsync_MovieExists_ReturnsMovieDetailsDto()
    {
        var userId = Guid.NewGuid();
        var movieId = Guid.NewGuid();
        var movie = Movie.Create(userId, "Inception", TimeSpan.FromMinutes(148), new DateOnly(2010, 7, 16));
        var movieDetailsDto = new MovieDetailsDto { Id = movieId, Title = "Inception" };
        
        _mockMovieRepository
            .Setup(r => r.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);
        
        _mockMapper
            .Setup(m => m.Map<MovieDetailsDto>(movie))
            .Returns(movieDetailsDto);
        
        var result = await _movieService.GetMovieByIdAsync(userId, movieId);
        
        Assert.NotNull(result);
        Assert.Equal("Inception", result.Title);
    }
    
    [Fact]
    public async Task GetMovieByIdAsync_WrongUser_ThrowsForbiddenException()
    {
        var userId = Guid.NewGuid();
        var wrongUserId = Guid.NewGuid();
        var movieId = Guid.NewGuid();
        var movie = Movie.Create(userId, "Inception", TimeSpan.FromMinutes(148), new DateOnly(2010, 7, 16));
        
        _mockMovieRepository
            .Setup(r => r.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);
        
        await Assert.ThrowsAsync<ForbiddenException>(
            () => _movieService.GetMovieByIdAsync(wrongUserId, movieId));
    }
    
    [Fact]
    public async Task CreateMovieAsync_ValidData_ReturnsMovieDetailsDto()
    {
        var userId = Guid.NewGuid();
        var createDto = new CreateMovieDto
        {
            Title = "Inception",
            Duration = 148,
            ReleaseDate = new DateOnly(2010, 7, 16)
        };
        
        var movieDetailsDto = new MovieDetailsDto { Title = "Inception" };
        
        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        _mockMapper
            .Setup(m => m.Map<MovieDetailsDto>(It.IsAny<Movie>()))
            .Returns(movieDetailsDto);
        
        var result = await _movieService.CreateMovieAsync(userId, createDto);
        
        Assert.NotNull(result);
        Assert.Equal("Inception", result.Title);
        _mockMovieRepository.Verify(r => r.AddAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task DeleteMovieAsync_MovieExists_DeletesMovie()
    {
        var userId = Guid.NewGuid();
        var movieId = Guid.NewGuid();
        var movie = Movie.Create(userId, "Inception", TimeSpan.FromMinutes(148), new DateOnly(2010, 7, 16));
        
        _mockMovieRepository
            .Setup(r => r.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);
        
        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        await _movieService.DeleteMovieAsync(userId, movieId);
        
        _mockMovieRepository.Verify(r => r.Delete(movie), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task DeleteMovieAsync_MovieNotFound_ThrowsNotFoundException()
    {
        var userId = Guid.NewGuid();
        var movieId = Guid.NewGuid();
        
        _mockMovieRepository
            .Setup(r => r.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Movie?)null);
        
        await Assert.ThrowsAsync<NotFoundException>(
            () => _movieService.DeleteMovieAsync(userId, movieId));
    }
}