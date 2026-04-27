using Miru.Application.Exceptions;
using Miru.Application.Interfaces;
using Miru.Application.Services;
using Miru.Domain.Entities;
using Miru.Domain.Repositories;
using Miru.Infrastructure.Persistence.UnitOfWork;
using Miru.Shared.Common;
using Miru.Shared.DTOs.Episodes;
using Moq;

namespace Miru.Test.Application;

public class EpisodeServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IEpisodeRepository> _mockEpisodeRepo;
    private readonly Mock<ISeasonRepository> _mockSeasonRepo;
    private readonly Mock<ISerieRepository> _mockSerieRepo;
    private readonly EpisodeService _episodeService;
    private readonly Guid _userId;

    public EpisodeServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
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

        _episodeService = new EpisodeService(_mockUnitOfWork.Object, mockCurrentUser.Object);
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

        _mockSerieRepo
            .Setup(r => r.GetByIdAsync(serieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);
        _mockSeasonRepo
            .Setup(r => r.GetByIdAsync(seasonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(season);
        _mockEpisodeRepo
            .Setup(r => r.GetBySeasonIdAsync(seasonId, EpisodeOrderingCriteria.ByEpisodeNumber, 0, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagingResult<Episode>
            {
                TotalCount = 2, PageIndex = 0, CountPerPage = 10, Items = episodes
            });

        var result = await _episodeService.GetEpisodesBySeasonIdAsync(serieId, seasonId, EpisodeOrderingCriteria.ByEpisodeNumber, 0, 10);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal("Ep1", result.Items.First().Title);
        Assert.Equal(45, result.Items.First().Duration);
    }

    [Fact]
    public async Task GetEpisodesBySeasonIdAsync_WrongUser_ThrowsForbidden()
    {
        var serieId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();
        var serie = Serie.Create(Guid.NewGuid(), "Serie", new DateOnly(2024, 1, 1));

        _mockSerieRepo
            .Setup(r => r.GetByIdAsync(serieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _episodeService.GetEpisodesBySeasonIdAsync(serieId, seasonId));
    }

    [Fact]
    public async Task GetEpisodeByIdAsync_ReturnsEpisodeDto_WhenEpisodeExists()
    {
        var serieId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();
        var serie = Serie.Create(_userId, "Serie", new DateOnly(2024, 1, 1));
        var season = Season.Create(1, new DateOnly(2024, 1, 1), serieId);
        var episode = Episode.Create(1, "Ep1", TimeSpan.FromMinutes(45), seasonId);

        _mockSerieRepo
            .Setup(r => r.GetByIdAsync(serieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);
        _mockSeasonRepo
            .Setup(r => r.GetByIdAsync(seasonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(season);
        _mockEpisodeRepo
            .Setup(r => r.GetByIdAsync(episode.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(episode);

        var result = await _episodeService.GetEpisodeByIdAsync(serieId, seasonId, episode.Id);

        Assert.NotNull(result);
        Assert.Equal("Ep1", result.Title);
        Assert.Equal(45, result.Duration);
        Assert.Equal(1, result.EpisodeNumber);
    }

    [Fact]
    public async Task GetEpisodeByIdAsync_WrongUser_ThrowsForbiddenException()
    {
        var serieId = Guid.NewGuid();
        var serie = Serie.Create(Guid.NewGuid(), "Serie", new DateOnly(2024, 1, 1));

        _mockSerieRepo
            .Setup(r => r.GetByIdAsync(serieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _episodeService.GetEpisodeByIdAsync(serieId, Guid.NewGuid(), Guid.NewGuid()));
    }

    [Fact]
    public async Task GetEpisodeByIdAsync_EpisodeNotFound_ReturnsNull()
    {
        var serieId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();
        var serie = Serie.Create(_userId, "Serie", new DateOnly(2024, 1, 1));
        var season = Season.Create(1, new DateOnly(2024, 1, 1), serieId);

        _mockSerieRepo
            .Setup(r => r.GetByIdAsync(serieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);
        _mockSeasonRepo
            .Setup(r => r.GetByIdAsync(seasonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(season);
        _mockEpisodeRepo
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Episode?)null);

        var result = await _episodeService.GetEpisodeByIdAsync(serieId, seasonId, Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task AddEpisodeToSeasonAsync_AddsEpisodeSuccessfully()
    {
        var serieId = Guid.NewGuid();
        var serie = Serie.Create(_userId, "Serie", new DateOnly(2024, 1, 1));
        var season = Season.Create(1, new DateOnly(2024, 1, 1), serieId);

        _mockSerieRepo
            .Setup(r => r.GetByIdAsync(serieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);
        _mockSeasonRepo
            .Setup(r => r.GetByIdWithEpisodesAsync(season.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(season);
        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var createDto = new CreateEpisodeDto { EpisodeNumber = 1, Title = "Ep1", DurationMinutes = 45 };

        var result = await _episodeService.AddEpisodeToSeasonAsync(serieId, season.Id, createDto);

        Assert.Equal("Ep1", result.Title);
        Assert.Equal(45, result.Duration);
        Assert.Equal(1, result.EpisodeNumber);
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
        season.AddEpisode(Episode.Create(1, "Ep1", TimeSpan.FromMinutes(45), season.Id));

        _mockSerieRepo
            .Setup(r => r.GetByIdAsync(serieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);
        _mockSeasonRepo
            .Setup(r => r.GetByIdWithEpisodesAsync(season.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(season);

        var createDto = new CreateEpisodeDto { EpisodeNumber = 1, Title = "Ep1", DurationMinutes = 45 };

        await Assert.ThrowsAsync<ValidationException>(
            () => _episodeService.AddEpisodeToSeasonAsync(serieId, season.Id, createDto));
    }

    [Fact]
    public async Task UpdateEpisodeAsync_UpdatesFieldsSuccessfully()
    {
        var serieId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();
        var serie = Serie.Create(_userId, "Serie", new DateOnly(2024, 1, 1));
        var season = Season.Create(1, new DateOnly(2024, 1, 1), serieId);
        var episode = Episode.Create(1, "Ep1", TimeSpan.FromMinutes(45), seasonId);

        _mockSerieRepo
            .Setup(r => r.GetByIdAsync(serieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);
        _mockSeasonRepo
            .Setup(r => r.GetByIdAsync(seasonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(season);
        _mockEpisodeRepo
            .Setup(r => r.GetByIdAsync(episode.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(episode);
        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var updateDto = new UpdateEpisodeDto { Title = "Updated", DurationMinutes = 50, EpisodeNumber = 2 };

        var result = await _episodeService.UpdateEpisodeAsync(serieId, seasonId, episode.Id, updateDto);

        Assert.Equal("Updated", result.Title);
        Assert.Equal(50, result.Duration);
        Assert.Equal(2, result.EpisodeNumber);
        Assert.Equal(2, episode.EpisodeNumber);
        Assert.Equal(TimeSpan.FromMinutes(50), episode.Duration);
        _mockEpisodeRepo.Verify(r => r.Update(episode), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateEpisodeAsync_EpisodeNotFound_ThrowsNotFoundException()
    {
        var serieId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();
        var serie = Serie.Create(_userId, "Serie", new DateOnly(2024, 1, 1));
        var season = Season.Create(1, new DateOnly(2024, 1, 1), serieId);

        _mockSerieRepo
            .Setup(r => r.GetByIdAsync(serieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);
        _mockSeasonRepo
            .Setup(r => r.GetByIdAsync(seasonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(season);
        _mockEpisodeRepo
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Episode?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _episodeService.UpdateEpisodeAsync(serieId, seasonId, Guid.NewGuid(), new UpdateEpisodeDto()));
    }

    [Fact]
    public async Task RateEpisodeAsync_ValidRating_UpdatesAndSaves()
    {
        var serieId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();
        var serie = Serie.Create(_userId, "Serie", new DateOnly(2024, 1, 1));
        var season = Season.Create(1, new DateOnly(2024, 1, 1), serieId);
        var episode = Episode.Create(1, "Ep1", TimeSpan.FromMinutes(45), seasonId);

        _mockSerieRepo
            .Setup(r => r.GetByIdAsync(serieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);
        _mockSeasonRepo
            .Setup(r => r.GetByIdAsync(seasonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(season);
        _mockEpisodeRepo
            .Setup(r => r.GetByIdAsync(episode.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(episode);
        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _episodeService.RateEpisodeAsync(serieId, seasonId, episode.Id, 8.5);

        Assert.Equal(8.5, episode.Rating);
        _mockEpisodeRepo.Verify(r => r.Update(episode), Times.Once);
    }

    [Fact]
    public async Task MarkEpisodeAsWatchedAsync_MarksEpisodeWatched()
    {
        var serieId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();
        var serie = Serie.Create(_userId, "Serie", new DateOnly(2024, 1, 1));
        var season = Season.Create(1, new DateOnly(2024, 1, 1), serieId);
        var episode = Episode.Create(1, "Ep1", TimeSpan.FromMinutes(45), seasonId);

        _mockSerieRepo
            .Setup(r => r.GetByIdAsync(serieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);
        _mockSeasonRepo
            .Setup(r => r.GetByIdAsync(seasonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(season);
        _mockEpisodeRepo
            .Setup(r => r.GetByIdAsync(episode.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(episode);
        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _episodeService.MarkEpisodeAsWatchedAsync(serieId, seasonId, episode.Id);

        Assert.True(episode.Watched);
        Assert.NotNull(episode.WatchedAt);
        _mockEpisodeRepo.Verify(r => r.Update(episode), Times.Once);
    }

    [Fact]
    public async Task MarkEpisodeAsUnwatchedAsync_MarksEpisodeUnwatched()
    {
        var serieId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();
        var serie = Serie.Create(_userId, "Serie", new DateOnly(2024, 1, 1));
        var season = Season.Create(1, new DateOnly(2024, 1, 1), serieId);
        var episode = Episode.Create(1, "Ep1", TimeSpan.FromMinutes(45), seasonId);
        episode.MarkAsWatched();

        _mockSerieRepo
            .Setup(r => r.GetByIdAsync(serieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);
        _mockSeasonRepo
            .Setup(r => r.GetByIdAsync(seasonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(season);
        _mockEpisodeRepo
            .Setup(r => r.GetByIdAsync(episode.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(episode);
        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _episodeService.MarkEpisodeAsUnwatchedAsync(serieId, seasonId, episode.Id);

        Assert.False(episode.Watched);
        Assert.Null(episode.WatchedAt);
        _mockEpisodeRepo.Verify(r => r.Update(episode), Times.Once);
    }

    [Fact]
    public async Task DeleteEpisodeAsync_RemovesEpisode()
    {
        var serieId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();
        var serie = Serie.Create(_userId, "Serie", new DateOnly(2024, 1, 1));
        var season = Season.Create(1, new DateOnly(2024, 1, 1), serieId);
        var episode = Episode.Create(1, "Ep1", TimeSpan.FromMinutes(45), seasonId);

        _mockSerieRepo
            .Setup(r => r.GetByIdAsync(serieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);
        _mockSeasonRepo
            .Setup(r => r.GetByIdAsync(seasonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(season);
        _mockEpisodeRepo
            .Setup(r => r.GetByIdAsync(episode.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(episode);
        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _episodeService.DeleteEpisodeAsync(serieId, seasonId, episode.Id);

        _mockEpisodeRepo.Verify(r => r.Delete(episode), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteEpisodeAsync_EpisodeNotFound_ThrowsNotFoundException()
    {
        var serieId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();
        var serie = Serie.Create(_userId, "Serie", new DateOnly(2024, 1, 1));
        var season = Season.Create(1, new DateOnly(2024, 1, 1), serieId);

        _mockSerieRepo
            .Setup(r => r.GetByIdAsync(serieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);
        _mockSeasonRepo
            .Setup(r => r.GetByIdAsync(seasonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(season);
        _mockEpisodeRepo
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Episode?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _episodeService.DeleteEpisodeAsync(serieId, seasonId, Guid.NewGuid()));
    }
}