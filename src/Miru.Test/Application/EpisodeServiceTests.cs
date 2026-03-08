using AutoMapper;
using Miru.Application.Exceptions;
using Miru.Application.Services;
using Miru.Domain.Entities;
using Miru.Domain.Repositories;
using Miru.Infrastructure.Persistence.UnitOfWork;
using Miru.Shared.Common;
using Miru.Shared.DTOs.Episodes;
using Miru.Shared.Services;
using Moq;

namespace Miru.Test.Application;

public class EpisodeServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IEpisodeRepository> _mockEpisodeRepo;
    private readonly Mock<ISeasonRepository> _mockSeasonRepo;
    private readonly Mock<ISerieRepository> _mockSerieRepo;
    private readonly EpisodeService _episodeService;
    private readonly Guid _userId;

    public EpisodeServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockEpisodeRepo = new Mock<IEpisodeRepository>();
        _mockSeasonRepo = new Mock<ISeasonRepository>();
        _mockSerieRepo = new Mock<ISerieRepository>();
        var mockCurrentUser = new Mock<ICurrentUserService>();

        _userId = Guid.NewGuid();

        mockCurrentUser
            .Setup(x => x.UserId)
            .Returns(_userId);

        _mockUnitOfWork.Setup(u => u.Episodes).Returns(_mockEpisodeRepo.Object);
        _mockUnitOfWork.Setup(u => u.Seasons).Returns(_mockSeasonRepo.Object);
        _mockUnitOfWork.Setup(u => u.Series).Returns(_mockSerieRepo.Object);

        _episodeService = new EpisodeService(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            mockCurrentUser.Object);
    }

    [Fact]
    public async Task GetEpisodesBySeasonIdAsync_ReturnsMappedPagingResult()
    {
        var serieId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();

        var serie = Serie.Create(_userId, "Serie", new DateOnly(2024, 1, 1));
        var season = Season.Create(1, new DateOnly(2024, 1, 1), serieId);

        var episodes = new List<Episode>
        {
            Episode.Create(1, "Ep1", TimeSpan.FromMinutes(45), seasonId),
            Episode.Create(2, "Ep2", TimeSpan.FromMinutes(50), seasonId)
        };

        var pagingResult = new PagingResult<Episode>
        {
            TotalCount = 2,
            PageIndex = 0,
            CountPerPage = 10,
            Items = episodes
        };

        _mockSerieRepo.Setup(r => r.GetByIdAsync(serieId, It.IsAny<CancellationToken>())).ReturnsAsync(serie);
        _mockSeasonRepo.Setup(r => r.GetByIdAsync(seasonId, It.IsAny<CancellationToken>())).ReturnsAsync(season);
        _mockEpisodeRepo.Setup(r => r.GetBySeasonIdAsync(seasonId, EpisodeOrderingCriteria.ByEpisodeNumber, 0, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagingResult);

        _mockMapper.Setup(m => m.Map<IEnumerable<EpisodeDto>>(episodes))
            .Returns(episodes.Select(e => new EpisodeDto { Id = e.Id, Title = e.Title }));

        var result = await _episodeService.GetEpisodesBySeasonIdAsync(serieId, seasonId, EpisodeOrderingCriteria.ByEpisodeNumber, 0, 10);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
    }

    [Fact]
    public async Task GetEpisodeByIdAsync_ReturnsEpisodeDto_WhenEpisodeExists()
    {
        var serieId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();

        var serie = Serie.Create(_userId, "Serie", new DateOnly(2024, 1, 1));
        var season = Season.Create(1, new DateOnly(2024, 1, 1), serieId);
        var episode = Episode.Create(1, "Ep1", TimeSpan.FromMinutes(45), seasonId);

        _mockSerieRepo.Setup(r => r.GetByIdAsync(serieId, It.IsAny<CancellationToken>())).ReturnsAsync(serie);
        _mockSeasonRepo.Setup(r => r.GetByIdAsync(seasonId, It.IsAny<CancellationToken>())).ReturnsAsync(season);
        _mockEpisodeRepo.Setup(r => r.GetByIdAsync(episode.Id, It.IsAny<CancellationToken>())).ReturnsAsync(episode);

        _mockMapper.Setup(m => m.Map<EpisodeDto>(episode)).Returns(new EpisodeDto { Id = episode.Id, Title = episode.Title });

        var result = await _episodeService.GetEpisodeByIdAsync(serieId, seasonId, episode.Id);

        Assert.NotNull(result);
        Assert.Equal("Ep1", result.Title);
    }

    [Fact]
    public async Task GetEpisodeByIdAsync_WrongUser_ThrowsForbiddenException()
    {
        var serieId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();

        var serie = Serie.Create(Guid.NewGuid(), "Serie", new DateOnly(2024, 1, 1));
        _mockSerieRepo.Setup(r => r.GetByIdAsync(serieId, It.IsAny<CancellationToken>())).ReturnsAsync(serie);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _episodeService.GetEpisodeByIdAsync(serieId, seasonId, Guid.NewGuid()));
    }

    [Fact]
    public async Task AddEpisodeToSeasonAsync_AddsEpisodeSuccessfully()
    {
        var serieId = Guid.NewGuid();

        var serie = Serie.Create(_userId, "Serie", new DateOnly(2024, 1, 1));
        var season = Season.Create(1, new DateOnly(2024, 1, 1), serieId);
        var seasonId = season.Id;

        _mockSerieRepo.Setup(r => r.GetByIdAsync(serieId, It.IsAny<CancellationToken>())).ReturnsAsync(serie);
        _mockSeasonRepo.Setup(r => r.GetByIdWithEpisodesAsync(seasonId, It.IsAny<CancellationToken>())).ReturnsAsync(season);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _mockMapper.Setup(m => m.Map<EpisodeDto>(It.IsAny<Episode>()))
            .Returns<Episode>(ep => new EpisodeDto { Id = ep.Id, Title = ep.Title });

        var createDto = new CreateEpisodeDto { EpisodeNumber = 1, Title = "Ep1", DurationMinutes = 45 };

        var result = await _episodeService.AddEpisodeToSeasonAsync(serieId, seasonId, createDto);

        Assert.Equal("Ep1", result.Title);
        Assert.Single(season.Episodes);
        _mockSeasonRepo.Verify(r => r.Update(season), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddEpisodeToSeasonAsync_DuplicateEpisode_ThrowsValidationException()
    {
        var serieId = Guid.NewGuid();

        var serie = Serie.Create(_userId, "Serie", new DateOnly(2024, 1, 1));
        var season = Season.Create(1, new DateOnly(2024, 1, 1), serieId);
        var seasonId = season.Id;

        var ep = Episode.Create(1, "Ep1", TimeSpan.FromMinutes(45), seasonId);
        season.AddEpisode(ep);

        _mockSerieRepo.Setup(r => r.GetByIdAsync(serieId, It.IsAny<CancellationToken>())).ReturnsAsync(serie);
        _mockSeasonRepo.Setup(r => r.GetByIdWithEpisodesAsync(seasonId, It.IsAny<CancellationToken>())).ReturnsAsync(season);

        var createDto = new CreateEpisodeDto { EpisodeNumber = 1, Title = "Ep1", DurationMinutes = 45 };

        await Assert.ThrowsAsync<ValidationException>(() =>
            _episodeService.AddEpisodeToSeasonAsync(serieId, seasonId, createDto));
    }

    [Fact]
    public async Task UpdateEpisodeAsync_UpdatesFieldsSuccessfully()
    {
        var serieId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();

        var serie = Serie.Create(_userId, "Serie", new DateOnly(2024, 1, 1));
        var season = Season.Create(1, new DateOnly(2024, 1, 1), serieId);
        var episode = Episode.Create(1, "Ep1", TimeSpan.FromMinutes(45), seasonId);

        _mockSerieRepo.Setup(r => r.GetByIdAsync(serieId, It.IsAny<CancellationToken>())).ReturnsAsync(serie);
        _mockSeasonRepo.Setup(r => r.GetByIdAsync(seasonId, It.IsAny<CancellationToken>())).ReturnsAsync(season);
        _mockEpisodeRepo.Setup(r => r.GetByIdAsync(episode.Id, It.IsAny<CancellationToken>())).ReturnsAsync(episode);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _mockMapper.Setup(m => m.Map<EpisodeDto>(episode)).Returns(new EpisodeDto { Id = episode.Id, Title = "Updated" });

        var updateDto = new UpdateEpisodeDto { Title = "Updated", DurationMinutes = 50, EpisodeNumber = 2 };

        var result = await _episodeService.UpdateEpisodeAsync(serieId, seasonId, episode.Id, updateDto);

        Assert.Equal("Updated", result.Title);
        Assert.Equal(2, episode.EpisodeNumber);
        Assert.Equal(TimeSpan.FromMinutes(50), episode.Duration);
    }

    [Fact]
    public async Task DeleteEpisodeAsync_RemovesEpisode()
    {
        var serieId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();
        var episode = Episode.Create(1, "Ep1", TimeSpan.FromMinutes(45), seasonId);

        var serie = Serie.Create(_userId, "Serie", new DateOnly(2024, 1, 1));
        var season = Season.Create(1, new DateOnly(2024, 1, 1), serieId);

        _mockSerieRepo.Setup(r => r.GetByIdAsync(serieId, It.IsAny<CancellationToken>())).ReturnsAsync(serie);
        _mockSeasonRepo.Setup(r => r.GetByIdAsync(seasonId, It.IsAny<CancellationToken>())).ReturnsAsync(season);
        _mockEpisodeRepo.Setup(r => r.GetByIdAsync(episode.Id, It.IsAny<CancellationToken>())).ReturnsAsync(episode);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _episodeService.DeleteEpisodeAsync(serieId, seasonId, episode.Id);

        _mockEpisodeRepo.Verify(r => r.Delete(episode), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteEpisodeAsync_EpisodeNotFound_ThrowsNotFoundException()
    {
        var seasonId = Guid.NewGuid();
        var episodeId = Guid.NewGuid();

        var serie = Serie.Create(_userId, "Serie", new DateOnly(2024, 1, 1));
        var serieId = serie.Id;
        var season = Season.Create(1, new DateOnly(2024, 1, 1), serie.Id);

        _mockSerieRepo.Setup(r => r.GetByIdAsync(serieId, It.IsAny<CancellationToken>())).ReturnsAsync(serie);
        _mockSeasonRepo.Setup(r => r.GetByIdAsync(seasonId, It.IsAny<CancellationToken>())).ReturnsAsync(season);
        _mockEpisodeRepo.Setup(r => r.GetByIdAsync(episodeId, It.IsAny<CancellationToken>())).ReturnsAsync((Episode?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _episodeService.DeleteEpisodeAsync(serieId, seasonId, episodeId));
    }
}