using Miru.Application.Exceptions;
using Miru.Application.Interfaces;
using Miru.Application.Services;
using Miru.Domain.Entities;
using Miru.Domain.Repositories;
using Miru.Infrastructure.Persistence.UnitOfWork;
using Miru.Shared.Common;
using Miru.Shared.Common.Enums;
using Miru.Shared.DTOs.Series;
using Moq;

namespace Miru.Test.Application;

public class SerieServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ISerieRepository> _mockSerieRepository;
    private readonly SerieService _serieService;
    private readonly Guid _userId;

    public SerieServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockSerieRepository = new Mock<ISerieRepository>();
        var mockCurrentUser = new Mock<ICurrentUserService>();
        _userId = Guid.NewGuid();

        mockCurrentUser
            .Setup(x => x.UserId)
            .Returns(_userId);

        _mockUnitOfWork.Setup(u => u.Series).Returns(_mockSerieRepository.Object);

        _serieService = new SerieService(_mockUnitOfWork.Object, mockCurrentUser.Object);
    }

    [Fact]
    public async Task GetSeriesAsync_ReturnsMappedPagingResult()
    {
        var series = new List<Serie>
        {
            Serie.Create(_userId, "Serie 1", new DateOnly(2024, 1, 1)),
            Serie.Create(_userId, "Serie 2", new DateOnly(2024, 1, 2))
        };

        _mockSerieRepository
            .Setup(r => r.GetSeriesAsync(_userId, SerieOrderingCriteria.None, 0, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagingResult<Serie>
            {
                TotalCount = 2, PageIndex = 0, CountPerPage = 10, Items = series
            });

        var result = await _serieService.GetSeriesAsync();

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal("Serie 1", result.Items.First().Title);
        Assert.Equal("Serie 2", result.Items.Last().Title);
        _mockSerieRepository.Verify(r => r.GetSeriesAsync(
            _userId, SerieOrderingCriteria.None, 0, 10, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetSerieByIdAsync_ReturnsSerieDetails_WhenUserMatches()
    {
        var serie = Serie.Create(_userId, "My Serie", new DateOnly(2024, 1, 1));

        _mockSerieRepository
            .Setup(r => r.GetByIdWithSeasonsAndEpisodesAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);

        var result = await _serieService.GetSerieByIdAsync(serie.Id);

        Assert.NotNull(result);
        Assert.Equal("My Serie", result.Title);
    }

    [Fact]
    public async Task GetSerieByIdAsync_WrongUser_ThrowsForbidden()
    {
        var serie = Serie.Create(Guid.NewGuid(), "My Serie", new DateOnly(2024, 1, 1));

        _mockSerieRepository
            .Setup(r => r.GetByIdWithSeasonsAndEpisodesAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _serieService.GetSerieByIdAsync(serie.Id));
    }

    [Fact]
    public async Task GetSerieByIdAsync_NotFound_ReturnsNull()
    {
        _mockSerieRepository
            .Setup(r => r.GetByIdWithSeasonsAndEpisodesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Serie?)null);

        var result = await _serieService.GetSerieByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateSerieAsync_ValidData_ReturnsDtoAndSaves()
    {
        var createDto = new CreateSerieDto
        {
            Title = "New Serie",
            ReleaseDate = new DateOnly(2024, 1, 1),
            Description = "Desc",
            PosterUrl = "https://poster.jpg"
        };

        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _serieService.CreateSerieAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal("New Serie", result.Title);
        Assert.Equal("Desc", result.Description);
        _mockSerieRepository.Verify(r => r.AddAsync(It.IsAny<Serie>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateSerieAsync_ValidData_UpdatesPropertiesAndMaps()
    {
        var serie = Serie.Create(_userId, "Old", new DateOnly(2023, 1, 1));
        var updateDto = new UpdateSerieDto { Title = "Updated", Description = "Desc" };

        _mockSerieRepository
            .Setup(r => r.GetByIdAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);
        _mockSerieRepository
            .Setup(r => r.GetByIdWithSeasonsAndEpisodesAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);
        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _serieService.UpdateSerieAsync(serie.Id, updateDto);

        Assert.Equal("Updated", result.Title);
        Assert.Equal("Desc", result.Description);
        _mockSerieRepository.Verify(r => r.Update(serie), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateSerieAsync_WrongUser_ThrowsForbidden()
    {
        var serie = Serie.Create(Guid.NewGuid(), "Serie", new DateOnly(2023, 1, 1));

        _mockSerieRepository
            .Setup(r => r.GetByIdAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _serieService.UpdateSerieAsync(serie.Id, new UpdateSerieDto()));
    }

    [Fact]
    public async Task UpdateSerieAsync_NotFound_ThrowsNotFound()
    {
        _mockSerieRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Serie?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _serieService.UpdateSerieAsync(Guid.NewGuid(), new UpdateSerieDto()));
    }

    [Fact]
    public async Task DeleteSerieAsync_DeletesSerie_WhenExists()
    {
        var serie = Serie.Create(_userId, "Serie", new DateOnly(2024, 1, 1));

        _mockSerieRepository
            .Setup(r => r.GetByIdAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);
        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _serieService.DeleteSerieAsync(serie.Id);

        _mockSerieRepository.Verify(r => r.Delete(serie), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteSerieAsync_NotFound_ThrowsNotFound()
    {
        _mockSerieRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Serie?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _serieService.DeleteSerieAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task DeleteSerieAsync_WrongUser_ThrowsForbidden()
    {
        var serie = Serie.Create(Guid.NewGuid(), "Serie", new DateOnly(2024, 1, 1));

        _mockSerieRepository
            .Setup(r => r.GetByIdAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _serieService.DeleteSerieAsync(serie.Id));
    }

    [Fact]
    public async Task UpdateSerieStatusAsync_ValidStatus_UpdatesAndSaves()
    {
        var serie = Serie.Create(_userId, "Serie", new DateOnly(2024, 1, 1));

        _mockSerieRepository
            .Setup(r => r.GetByIdAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);
        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _serieService.UpdateSerieStatusAsync(serie.Id, "Watched");

        Assert.Equal(MediaStatus.Watched, serie.Status);
        _mockSerieRepository.Verify(r => r.Update(serie), Times.Once);
    }

    [Fact]
    public async Task UpdateSerieStatusAsync_InvalidStatus_ThrowsValidation()
    {
        await Assert.ThrowsAsync<ValidationException>(
            () => _serieService.UpdateSerieStatusAsync(Guid.NewGuid(), "InvalidStatus"));
    }

    [Fact]
    public async Task RateSerieAsync_ValidRating_UpdatesAndSaves()
    {
        var serie = Serie.Create(_userId, "Serie", new DateOnly(2024, 1, 1));

        _mockSerieRepository
            .Setup(r => r.GetByIdAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);
        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _serieService.RateSerieAsync(serie.Id, 8.5);

        Assert.Equal(8.5, serie.Rating);
        _mockSerieRepository.Verify(r => r.Update(serie), Times.Once);
    }
}