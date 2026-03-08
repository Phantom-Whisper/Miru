using AutoMapper;
using Miru.Application.Exceptions;
using Miru.Application.Services;
using Miru.Domain.Entities;
using Miru.Domain.Repositories;
using Miru.Infrastructure.Persistence.UnitOfWork;
using Miru.Shared.DTOs.Seasons;
using Miru.Shared.Services;
using Moq;

namespace Miru.Test.Application;

public class SeasonServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ISerieRepository> _mockSerieRepository;
    private readonly Mock<ISeasonRepository> _mockSeasonRepository;
    private readonly Mock<IEpisodeRepository> _mockEpisodeRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Guid _userId;

    private readonly SeasonService _seasonService;

    public SeasonServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockSerieRepository = new Mock<ISerieRepository>();
        _mockSeasonRepository = new Mock<ISeasonRepository>();
        _mockEpisodeRepository = new Mock<IEpisodeRepository>();
        var mockCurrentUser = new Mock<ICurrentUserService>();
        _mockMapper = new Mock<IMapper>();
        _userId = Guid.NewGuid();

        mockCurrentUser
            .Setup(x => x.UserId)
            .Returns(_userId);

        _mockUnitOfWork.Setup(u => u.Series).Returns(_mockSerieRepository.Object);
        _mockUnitOfWork.Setup(u => u.Seasons).Returns(_mockSeasonRepository.Object);
        _mockUnitOfWork.Setup(u => u.Episodes).Returns(_mockEpisodeRepository.Object);

        _seasonService = new SeasonService(_mockUnitOfWork.Object, _mockMapper.Object, mockCurrentUser.Object);
    }

    [Fact]
    public async Task GetSeasonsBySerieIdAsync_ReturnsSeasons()
    {
        var serie = Serie.Create(_userId, "Breaking Bad", new DateOnly(2008, 1, 1));
        var seasons = new List<Season>
        {
            Season.Create(1, new DateOnly(2008,1,1), serie.Id),
            Season.Create(2, new DateOnly(2009,1,1), serie.Id)
        };

        var seasonDtos = seasons.Select(s => new SeasonDto { Id = s.Id }).ToList();

        _mockSerieRepository.Setup(r => r.GetByIdAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);

        _mockSeasonRepository.Setup(r => r.GetBySerieIdAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(seasons);

        _mockMapper.Setup(m => m.Map<IEnumerable<SeasonDto>>(seasons))
            .Returns(seasonDtos);

        var result = await _seasonService.GetSeasonsBySerieIdAsync(serie.Id);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetSeasonsBySerieIdAsync_WrongUser_ThrowsForbidden()
    {
        var serie = Serie.Create(Guid.NewGuid(), "BB", new DateOnly(2008,1,1));

        _mockSerieRepository.Setup(r => r.GetByIdAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _seasonService.GetSeasonsBySerieIdAsync(serie.Id));
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

        var seasonDetails = new SeasonDetailsDto
        {
            SeasonNumber = 1
        };

        _mockSerieRepository
            .Setup(r => r.GetByIdWithSeasonsAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);

        _mockSeasonRepository
            .Setup(r => r.GetByIdWithEpisodesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdSeason);

        _mockMapper
            .Setup(m => m.Map<SeasonDetailsDto>(It.IsAny<Season>()))
            .Returns(seasonDetails);

        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _seasonService.AddSeasonToSerieAsync(
            serie.Id,
            createDto,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.SeasonNumber);

        _mockSerieRepository.Verify(r => r.Update(serie), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddSeasonToSerieAsync_DuplicateSeason_ThrowsValidation()
    {
        var serie = Serie.Create(_userId, "Test", new DateOnly(2020, 1, 1));
        var existing = Season.Create(1, new DateOnly(2020,1,1), serie.Id);
        serie.AddSeason(existing);

        var dto = new CreateSeasonDto
        {
            SeasonNumber = 1,
            ReleaseDate = new DateOnly(2021,1,1)
        };

        _mockSerieRepository.Setup(r => r.GetByIdWithSeasonsAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);

        await Assert.ThrowsAsync<ValidationException>(
            () => _seasonService.AddSeasonToSerieAsync(serie.Id, dto));
    }
    
    [Fact]
    public async Task UpdateSeasonAsync_Valid_UpdatesSeason()
    {
        var serie = Serie.Create(_userId, "Dark", new DateOnly(2017,1,1));
        var season = Season.Create(1, new DateOnly(2017,1,1), serie.Id);

        var dto = new UpdateSeasonDto
        {
            SeasonNumber = 2
        };

        _mockSerieRepository.Setup(r => r.GetByIdAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);

        _mockSeasonRepository.Setup(r => r.GetByIdAsync(season.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(season);

        _mockSeasonRepository.Setup(r => r.GetByIdWithEpisodesAsync(season.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(season);

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _seasonService.UpdateSeasonAsync(serie.Id, season.Id, dto);

        Assert.Equal(2, season.SeasonNumber);
        _mockSeasonRepository.Verify(r => r.Update(season), Times.Once);
    }

    [Fact]
    public async Task DeleteSeasonAsync_Valid_DeletesSeason()
    {
        var serie = Serie.Create(_userId, "BB", new DateOnly(2008,1,1));
        var season = Season.Create(1, new DateOnly(2008,1,1), serie.Id);

        _mockSerieRepository.Setup(r => r.GetByIdAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);

        _mockSeasonRepository.Setup(r => r.GetByIdAsync(season.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(season);

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _seasonService.DeleteSeasonAsync(serie.Id, season.Id);

        _mockSeasonRepository.Verify(r => r.Delete(season), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkSeasonAsWatchedAsync_MarksAllEpisodes()
    {
        var serie = Serie.Create(_userId, "Test", new DateOnly(2020,1,1));
        var season = Season.Create(1, new DateOnly(2020,1,1), serie.Id);

        var episode1 = Episode.Create(1, "Ep1", TimeSpan.FromMinutes(40), season.Id);
        var episode2 = Episode.Create(2, "Ep2", TimeSpan.FromMinutes(40), season.Id);

        season.AddEpisode(episode1);
        season.AddEpisode(episode2);

        _mockSerieRepository.Setup(r => r.GetByIdAsync(serie.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serie);

        _mockSeasonRepository.Setup(r => r.GetByIdWithEpisodesAsync(season.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(season);

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _seasonService.MarkSeasonAsWatchedAsync(serie.Id, season.Id);

        Assert.True(episode1.Watched);
        Assert.True(episode2.Watched);

        _mockEpisodeRepository.Verify(r => r.Update(It.IsAny<Episode>()), Times.Exactly(2));
    }
}