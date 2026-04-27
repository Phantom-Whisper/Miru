using Miru.Application.Exceptions;
using Miru.Application.Interfaces;
using Miru.Application.Services;
using Miru.Domain.Entities;
using Miru.Domain.Repositories;
using Miru.Infrastructure.Persistence.UnitOfWork;
using Miru.Shared.Common;
using Miru.Shared.DTOs.Movies;
using Moq;

namespace Miru.Test.Application;

public class MovieServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMovieRepository> _mockMovieRepository;
    private readonly MovieService _movieService;
    private readonly Guid _userId;

    public MovieServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMovieRepository = new Mock<IMovieRepository>();
        var mockCurrentUser = new Mock<ICurrentUserService>();
        _userId = Guid.NewGuid();

        mockCurrentUser
            .Setup(x => x.UserId)
            .Returns(_userId);

        _mockUnitOfWork.Setup(u => u.Movies).Returns(_mockMovieRepository.Object);

        _movieService = new MovieService(_mockUnitOfWork.Object, mockCurrentUser.Object);
    }

    [Fact]
    public async Task GetUserMoviesAsync_ReturnsMovies()
    {
        var movies = new List<Movie>
        {
            Movie.Create(_userId, "Movie 1", TimeSpan.FromMinutes(120), new DateOnly(2024, 1, 1)),
            Movie.Create(_userId, "Movie 2", TimeSpan.FromMinutes(130), new DateOnly(2024, 1, 2))
        };

        _mockMovieRepository
            .Setup(r => r.GetMoviesAsync(
                _userId,
                MovieOrderingCriteria.None,
                0,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagingResult<Movie>
            {
                TotalCount = 2, PageIndex = 0, CountPerPage = 10, Items = movies
            });

        var result = await _movieService.GetMoviesAsync();

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal("Movie 1", result.Items.First().Title);
        Assert.Equal("Movie 2", result.Items.Last().Title);
        _mockMovieRepository.Verify(r => r.GetMoviesAsync(
            _userId,
            MovieOrderingCriteria.None,
            0,
            10,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetMovieByIdAsync_MovieExists_ReturnsMovieDetailsDto()
    {
        var movieId = Guid.NewGuid();
        var movie = Movie.Create(_userId, "Inception", TimeSpan.FromMinutes(148), new DateOnly(2010, 7, 16), "Dream infiltration", "https://poster.jpg");

        _mockMovieRepository
            .Setup(r => r.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);

        var result = await _movieService.GetMovieByIdAsync(movieId);

        Assert.NotNull(result);
        Assert.Equal("Inception", result.Title);
        Assert.Equal(148, result.Duration);
        Assert.Equal("Dream infiltration", result.Description);
        Assert.Equal("https://poster.jpg", result.PosterUrl);
    }

    [Fact]
    public async Task GetMovieByIdAsync_MovieNotFound_ReturnsNull()
    {
        var movieId = Guid.NewGuid();

        _mockMovieRepository
            .Setup(r => r.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Movie?)null);

        var result = await _movieService.GetMovieByIdAsync(movieId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetMovieByIdAsync_WrongUser_ThrowsForbiddenException()
    {
        var movieId = Guid.NewGuid();
        var movie = Movie.Create(Guid.NewGuid(), "Inception", TimeSpan.FromMinutes(148), new DateOnly(2010, 7, 16));

        _mockMovieRepository
            .Setup(r => r.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _movieService.GetMovieByIdAsync(movieId));
    }

    [Fact]
    public async Task CreateMovieAsync_ValidData_ReturnsMovieDetailsDto()
    {
        var createDto = new CreateMovieDto
        {
            Title = "Inception",
            Duration = 148,
            ReleaseDate = new DateOnly(2010, 7, 16),
            Description = "Dream infiltration",
            PosterUrl = "https://poster.jpg"
        };

        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _movieService.CreateMovieAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal("Inception", result.Title);
        Assert.Equal(148, result.Duration);
        Assert.Equal("Dream infiltration", result.Description);
        _mockMovieRepository.Verify(r => r.AddAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateMovieAsync_ValidData_ReturnsUpdatedMovieDetailsDto()
    {
        var movieId = Guid.NewGuid();
        var movie = Movie.Create(_userId, "Old Title", TimeSpan.FromMinutes(100), new DateOnly(2020, 1, 1));

        var updateDto = new UpdateMovieDto
        {
            Title = "New Title",
            Duration = 120,
        };

        _mockMovieRepository
            .Setup(r => r.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);

        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _movieService.UpdateMovieAsync(movieId, updateDto);

        Assert.NotNull(result);
        Assert.Equal("New Title", result.Title);
        Assert.Equal(120, result.Duration);
        _mockMovieRepository.Verify(r => r.Update(movie), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateMovieAsync_WrongUser_ThrowsForbiddenException()
    {
        var movieId = Guid.NewGuid();
        var movie = Movie.Create(Guid.NewGuid(), "Inception", TimeSpan.FromMinutes(148), new DateOnly(2010, 7, 16));

        _mockMovieRepository
            .Setup(r => r.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _movieService.UpdateMovieAsync(movieId, new UpdateMovieDto()));
    }

    [Fact]
    public async Task UpdateMovieAsync_NotFound_ThrowsNotFoundException()
    {
        _mockMovieRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Movie?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _movieService.UpdateMovieAsync(Guid.NewGuid(), new UpdateMovieDto()));
    }

    [Fact]
    public async Task DeleteMovieAsync_MovieExists_DeletesMovie()
    {
        var movieId = Guid.NewGuid();
        var movie = Movie.Create(_userId, "Inception", TimeSpan.FromMinutes(148), new DateOnly(2010, 7, 16));

        _mockMovieRepository
            .Setup(r => r.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);

        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _movieService.DeleteMovieAsync(movieId);

        _mockMovieRepository.Verify(r => r.Delete(movie), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteMovieAsync_MovieNotFound_ThrowsNotFoundException()
    {
        var movieId = Guid.NewGuid();

        _mockMovieRepository
            .Setup(r => r.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Movie?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _movieService.DeleteMovieAsync(movieId));
    }

    [Fact]
    public async Task DeleteMovieAsync_WrongUser_ThrowsForbiddenException()
    {
        var movieId = Guid.NewGuid();
        var movie = Movie.Create(Guid.NewGuid(), "Inception", TimeSpan.FromMinutes(148), new DateOnly(2010, 7, 16));

        _mockMovieRepository
            .Setup(r => r.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _movieService.DeleteMovieAsync(movieId));
    }
}