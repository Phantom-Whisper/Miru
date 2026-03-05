using AutoMapper;
using Miru.Application.Exceptions;
using Miru.Application.Services;
using Miru.Contracts.Common;
using Miru.Contracts.DTOs.Series;
using Miru.Contracts.Persistence;
using Miru.Contracts.Repositories;
using Miru.Contracts.Services;
using Miru.Domain;
using Miru.Domain.Entities;
using Moq;

namespace Miru.Test.Application;

public class SerieServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ISerieRepository> _mockSerieRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly SerieService _serieService;
    private readonly Guid _userId;

    public SerieServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockSerieRepository = new Mock<ISerieRepository>();
        _mockMapper = new Mock<IMapper>();
        
        var mockCurrentUser = new Mock<ICurrentUserService>();
        _userId = Guid.NewGuid();

        mockCurrentUser
            .Setup(x => x.UserId)
            .Returns(_userId);

        _mockUnitOfWork.Setup(u => u.Series).Returns(_mockSerieRepository.Object);

        _serieService = new SerieService(_mockUnitOfWork.Object, _mockMapper.Object, mockCurrentUser.Object);
    }

    [Fact]
    public async Task GetSeriesAsync_ReturnsMappedPagingResult()
    {
        var series = new List<Serie>
        {
            Serie.Create(_userId, "Serie 1", new DateOnly(2024,1,1)),
            Serie.Create(_userId, "Serie 2", new DateOnly(2024,1,2))
        };

        var pagingResult = new PagingResult<Serie>
        {
            TotalCount = 2,
            PageIndex = 0,
            CountPerPage = 10,
            Items = series
        };

        var serieDtos = series.Select(s => new SerieDto { Id = s.Id, Title = s.Title }).ToList();

        _mockSerieRepository
            .Setup(r => r.GetSeriesAsync(_userId, SerieOrderingCriteria.None, 0, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagingResult);

        _mockMapper
            .Setup(m => m.Map<IEnumerable<SerieDto>>(series))
            .Returns(serieDtos);

        var result = await _serieService.GetSeriesAsync();

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
        _mockSerieRepository.Verify(r => r.GetSeriesAsync(_userId, SerieOrderingCriteria.None, 0, 10, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetSerieByIdAsync_ReturnsSerieDetails_WhenUserMatches()
    {
        var serieId = Guid.NewGuid();
        var serie = Serie.Create(_userId, "My Serie", new DateOnly(2024,1,1));

        _mockSerieRepository.Setup(r => r.GetByIdWithSeasonsAndEpisodesAsync(serieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);

        var detailsDto = new SerieDetailsDto { Id = serieId, Title = "My Serie" };
        _mockMapper.Setup(m => m.Map<SerieDetailsDto>(serie)).Returns(detailsDto);

        var result = await _serieService.GetSerieByIdAsync(serieId);

        Assert.NotNull(result);
        Assert.Equal("My Serie", result.Title);
    }

    [Fact]
    public async Task GetSerieByIdAsync_WrongUser_ThrowsForbidden()
    {
        var serie = Serie.Create(Guid.NewGuid(), "My Serie", new DateOnly(2024,1,1));

        _mockSerieRepository.Setup(r => r.GetByIdWithSeasonsAndEpisodesAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);

        await Assert.ThrowsAsync<ForbiddenException>(() => _serieService.GetSerieByIdAsync(serie.Id));
    }

    [Fact]
    public async Task CreateSerieAsync_ValidData_ReturnsDtoAndSaves()
    {
        Guid.NewGuid();
        var createDto = new CreateSerieDto
        {
            Title = "New Serie",
            ReleaseDate = new DateOnly(2024,1,1),
            Description = "Desc",
            PosterUrl = "url"
        };

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var mappedDto = new SerieDetailsDto { Title = "New Serie" };
        _mockMapper.Setup(m => m.Map<SerieDetailsDto>(It.IsAny<Serie>())).Returns(mappedDto);

        var result = await _serieService.CreateSerieAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal("New Serie", result.Title);
        _mockSerieRepository.Verify(r => r.AddAsync(It.IsAny<Serie>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateSerieAsync_ValidData_UpdatesPropertiesAndMaps()
    {
        var serieId = Guid.NewGuid();
        var serie = Serie.Create(_userId, "Old", new DateOnly(2023,1,1));
        var updateDto = new UpdateSerieDto { Title = "Updated", Description = "Desc" };

        _mockSerieRepository.Setup(r => r.GetByIdAsync(serieId, It.IsAny<CancellationToken>())).ReturnsAsync(serie);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mockSerieRepository.Setup(r => r.GetByIdWithSeasonsAndEpisodesAsync(serieId, It.IsAny<CancellationToken>())).ReturnsAsync(serie);

        var mappedDto = new SerieDetailsDto { Title = "Updated" };
        _mockMapper.Setup(m => m.Map<SerieDetailsDto>(serie)).Returns(mappedDto);

        var result = await _serieService.UpdateSerieAsync(serieId, updateDto);

        Assert.Equal("Updated", result.Title);
        _mockSerieRepository.Verify(r => r.Update(serie), Times.Once);
    }

    [Fact]
    public async Task UpdateSerieAsync_WrongUser_ThrowsForbidden()
    {
        var serie = Serie.Create(Guid.NewGuid(), "Serie", new DateOnly(2023,1,1));
        _mockSerieRepository.Setup(r => r.GetByIdAsync(serie.Id, It.IsAny<CancellationToken>())).ReturnsAsync(serie);

        await Assert.ThrowsAsync<ForbiddenException>(() => _serieService.UpdateSerieAsync(serie.Id, new UpdateSerieDto()));
    }

    [Fact]
    public async Task UpdateSerieAsync_NotFound_ThrowsNotFound()
    {
        _mockSerieRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Serie?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _serieService.UpdateSerieAsync(Guid.NewGuid(), new UpdateSerieDto()));
    }

    [Fact]
    public async Task DeleteSerieAsync_DeletesSerie_WhenExists()
    {
        var serie = Serie.Create(_userId, "Serie", new DateOnly(2024,1,1));

        _mockSerieRepository.Setup(r => r.GetByIdAsync(serie.Id, It.IsAny<CancellationToken>())).ReturnsAsync(serie);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _serieService.DeleteSerieAsync(serie.Id);

        _mockSerieRepository.Verify(r => r.Delete(serie), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteSerieAsync_NotFound_ThrowsNotFound()
    {
        _mockSerieRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Serie?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _serieService.DeleteSerieAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task UpdateSerieStatusAsync_ValidStatus_UpdatesAndSaves()
    {
        var serie = Serie.Create(_userId, "Serie", new DateOnly(2024,1,1));
        _mockSerieRepository.Setup(r => r.GetByIdAsync(serie.Id, It.IsAny<CancellationToken>())).ReturnsAsync(serie);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _serieService.UpdateSerieStatusAsync(serie.Id, "Watched");

        Assert.Equal(MediaStatus.Watched, serie.Status);
        _mockSerieRepository.Verify(r => r.Update(serie), Times.Once);
    }

    [Fact]
    public async Task UpdateSerieStatusAsync_InvalidStatus_ThrowsValidation()
    {
        await Assert.ThrowsAsync<ValidationException>(() => _serieService.UpdateSerieStatusAsync(Guid.NewGuid(), "InvalidStatus"));
    }

    [Fact]
    public async Task RateSerieAsync_ValidRating_UpdatesAndSaves()
    {
        var serie = Serie.Create(_userId, "Serie", new DateOnly(2024,1,1));
        _mockSerieRepository.Setup(r => r.GetByIdAsync(serie.Id, It.IsAny<CancellationToken>())).ReturnsAsync(serie);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _serieService.RateSerieAsync(serie.Id, 8.5);

        Assert.Equal(8.5, serie.Rating);
        _mockSerieRepository.Verify(r => r.Update(serie), Times.Once);
    }
}