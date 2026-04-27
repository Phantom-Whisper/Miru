using Miru.Application.Exceptions;
using Miru.Application.Interfaces;
using Miru.Application.Services;
using Miru.Domain.Entities;
using Miru.Domain.Repositories;
using Miru.Infrastructure.Persistence.UnitOfWork;
using Miru.Shared.DTOs.Seasons;
using Moq;

namespace Miru.Test.Application;

public class SeasonServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ISerieRepository> _mockSerieRepository;
    private readonly Mock<ISeasonRepository> _mockSeasonRepository;
    private readonly Mock<IEpisodeRepository> _mockEpisodeRepository;
    private readonly SeasonService _seasonService;
    private readonly Guid _userId;

    public SeasonServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockSerieRepository = new Mock<ISerieRepository>();
        _mockSeasonRepository = new Mock<ISeasonRepository>();
        _mockEpisodeRepository = new Mock<IEpisodeRepository>();
        var mockCurrentUser = new Mock<ICurrentUserService>();
        _userId = Guid.NewGuid();

        mockCurrentUser
            .Setup(x => x.UserId)
            .Returns(_userId);

        _mockUnitOfWork.Setup(u => u.Series).Returns(_mockSerieRepository.Object);
        _mockUnitOfWork.Setup(u => u.Seasons).Returns(_mockSeasonRepository.Object);
        _mockUnitOfWork.Setup(u => u.Episodes).Returns(_mockEpisodeRepository.Object);

        _seasonService = new SeasonService(_mockUnitOfWork.Object, mockCurrentUser.Object);
    }

    [Fact]
    public async Task GetSeasonsBySerieIdAsync_ReturnsSeasons()
    {
        var serie = Serie.Create(_userId, "Breaking Bad", new DateOnly(2008, 1, 1));
        var seasons = new List<Season>
        {
            Season.Create(1, new DateOnly(2008, 1, 1), serie.Id),
            Season.Create(2, new DateOnly(2009, 1, 1), serie.Id)
        };

        _mockSerieRepository
            .Setup(r => r.GetByIdAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);
        _mockSeasonRepository
            .Setup(r => r.GetBySerieIdAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(seasons);

        var result = await _seasonService.GetSeasonsBySerieIdAsync(serie.Id);

        IEnumerable<SeasonDto> seasonDtos = result as SeasonDto[] ?? result.ToArray();
        Assert.Equal(2, seasonDtos.Count());
        Assert.Equal(1, seasonDtos.First().SeasonNumber);
        Assert.Equal(2, seasonDtos.Last().SeasonNumber);
    }

    [Fact]
    public async Task GetSeasonsBySerieIdAsync_WrongUser_ThrowsForbidden()
    {
        var serie = Serie.Create(Guid.NewGuid(), "BB", new DateOnly(2008, 1, 1));

        _mockSerieRepository
            .Setup(r => r.GetByIdAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _seasonService.GetSeasonsBySerieIdAsync(serie.Id));
    }

    [Fact]
    public async Task GetSeasonsBySerieIdAsync_SerieNotFound_ThrowsNotFoundException()
    {
        _mockSerieRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Serie?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _seasonService.GetSeasonsBySerieIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task AddSeasonToSerieAsync_Valid_ReturnsSeasonDetails()
    {
        var serie = Serie.Create(_userId, "Lost", new DateOnly(2004, 1, 1));
        var createDto = new CreateSeasonDto
        {
            SeasonNumber = 1,
            ReleaseDate = new DateOnly(2004, 9, 1)
        };

        var createdSeason = Season.Create(1, new DateOnly(2004, 9, 1), serie.Id);

        _mockSerieRepository
            .Setup(r => r.GetByIdWithSeasonsAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);

        _mockSeasonRepository
            .Setup(r => r.GetByIdWithEpisodesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdSeason);

        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _seasonService.AddSeasonToSerieAsync(serie.Id, createDto);

        Assert.NotNull(result);
        Assert.Equal(1, result.SeasonNumber);
        Assert.Equal(new DateOnly(2004, 9, 1), result.ReleaseDate);
        Assert.Empty(result.Episodes);
        _mockSerieRepository.Verify(r => r.Update(serie), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddSeasonToSerieAsync_DuplicateSeason_ThrowsValidation()
    {
        var serie = Serie.Create(_userId, "Test", new DateOnly(2020, 1, 1));
        var existing = Season.Create(1, new DateOnly(2020, 1, 1), serie.Id);
        serie.AddSeason(existing);

        var dto = new CreateSeasonDto { SeasonNumber = 1, ReleaseDate = new DateOnly(2021, 1, 1) };

        _mockSerieRepository
            .Setup(r => r.GetByIdWithSeasonsAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);

        await Assert.ThrowsAsync<ValidationException>(
            () => _seasonService.AddSeasonToSerieAsync(serie.Id, dto));
    }

    [Fact]
    public async Task AddSeasonToSerieAsync_WrongUser_ThrowsForbidden()
    {
        var serie = Serie.Create(Guid.NewGuid(), "Test", new DateOnly(2020, 1, 1));

        _mockSerieRepository
            .Setup(r => r.GetByIdWithSeasonsAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _seasonService.AddSeasonToSerieAsync(serie.Id, new CreateSeasonDto()));
    }

    [Fact]
    public async Task UpdateSeasonAsync_Valid_UpdatesSeason()
    {
        var serie = Serie.Create(_userId, "Dark", new DateOnly(2017, 1, 1));
        var season = Season.Create(1, new DateOnly(2017, 1, 1), serie.Id);
        var dto = new UpdateSeasonDto { SeasonNumber = 2 };

        _mockSerieRepository
            .Setup(r => r.GetByIdAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);
        _mockSeasonRepository
            .Setup(r => r.GetByIdAsync(season.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(season);
        _mockSeasonRepository
            .Setup(r => r.GetByIdWithEpisodesAsync(season.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(season);
        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _seasonService.UpdateSeasonAsync(serie.Id, season.Id, dto);

        Assert.Equal(2, result.SeasonNumber);
        Assert.Equal(2, season.SeasonNumber);
        _mockSeasonRepository.Verify(r => r.Update(season), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateSeasonAsync_WrongUser_ThrowsForbidden()
    {
        var serie = Serie.Create(Guid.NewGuid(), "Dark", new DateOnly(2017, 1, 1));
        var season = Season.Create(1, new DateOnly(2017, 1, 1), serie.Id);

        _mockSerieRepository
            .Setup(r => r.GetByIdAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _seasonService.UpdateSeasonAsync(serie.Id, season.Id, new UpdateSeasonDto()));
    }

    [Fact]
    public async Task UpdateSeasonAsync_SeasonNotFound_ThrowsNotFoundException()
    {
        var serie = Serie.Create(_userId, "Dark", new DateOnly(2017, 1, 1));

        _mockSerieRepository
            .Setup(r => r.GetByIdAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);
        _mockSeasonRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Season?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _seasonService.UpdateSeasonAsync(serie.Id, Guid.NewGuid(), new UpdateSeasonDto()));
    }

    [Fact]
    public async Task DeleteSeasonAsync_Valid_DeletesSeason()
    {
        var serie = Serie.Create(_userId, "BB", new DateOnly(2008, 1, 1));
        var season = Season.Create(1, new DateOnly(2008, 1, 1), serie.Id);

        _mockSerieRepository
            .Setup(r => r.GetByIdAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);
        _mockSeasonRepository
            .Setup(r => r.GetByIdAsync(season.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(season);
        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _seasonService.DeleteSeasonAsync(serie.Id, season.Id);

        _mockSeasonRepository.Verify(r => r.Delete(season), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteSeasonAsync_WrongUser_ThrowsForbidden()
    {
        var serie = Serie.Create(Guid.NewGuid(), "BB", new DateOnly(2008, 1, 1));
        var season = Season.Create(1, new DateOnly(2008, 1, 1), serie.Id);

        _mockSerieRepository
            .Setup(r => r.GetByIdAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _seasonService.DeleteSeasonAsync(serie.Id, season.Id));
    }

    [Fact]
    public async Task MarkSeasonAsWatchedAsync_MarksAllEpisodes()
    {
        var serie = Serie.Create(_userId, "Test", new DateOnly(2020, 1, 1));
        var season = Season.Create(1, new DateOnly(2020, 1, 1), serie.Id);
        var ep1 = Episode.Create(1, "Ep1", TimeSpan.FromMinutes(40), season.Id);
        var ep2 = Episode.Create(2, "Ep2", TimeSpan.FromMinutes(40), season.Id);
        season.AddEpisode(ep1);
        season.AddEpisode(ep2);

        _mockSerieRepository
            .Setup(r => r.GetByIdAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);
        _mockSeasonRepository
            .Setup(r => r.GetByIdWithEpisodesAsync(season.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(season);
        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _seasonService.MarkSeasonAsWatchedAsync(serie.Id, season.Id);

        Assert.True(ep1.Watched);
        Assert.True(ep2.Watched);
        _mockEpisodeRepository.Verify(r => r.Update(It.IsAny<Episode>()), Times.Exactly(2));
    }
}