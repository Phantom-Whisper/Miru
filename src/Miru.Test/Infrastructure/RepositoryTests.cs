using Microsoft.EntityFrameworkCore;
using Miru.Domain.Entities;
using Miru.Infrastructure;
using Miru.Infrastructure.Persistence.UnitOfWork;
using Miru.Shared.Common;
using Miru.Shared.Common.Enums;

namespace Miru.Test.Infrastructure;

public class RepositoryTests : IDisposable
{
    private readonly MiruDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public RepositoryTests()
    {
        var databaseName = $"MiruTestDb_{Guid.NewGuid()}";
        var connectionString = $"Server=(localdb)\\mssqllocaldb;Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=true";
        
        var options = new DbContextOptionsBuilder<MiruDbContext>()
            .UseSqlServer(connectionString)
            .EnableSensitiveDataLogging()
            .Options;
        
        _context = new MiruDbContext(options);
        _context.Database.EnsureCreated();
        
        _unitOfWork = new UnitOfWork(_context);
    }
    
    #region Movie Repository Tests
    
    [Fact]
    public async Task MovieRepository_CanAddAndRetrieveMovie()
    {
        var user = new UserEntity 
        { 
            UserName = "testuser", 
            Email = "test@example.com" 
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var movie = Movie.Create(
            user.Id,
            "Inception",
            TimeSpan.FromMinutes(148),
            new DateOnly(2010, 7, 16),
            "A thief who steals corporate secrets through dreams",
            "https://example.com/inception.jpg"
        );
        
        await _unitOfWork.Movies.AddAsync(movie);
        await _unitOfWork.SaveChangesAsync();
        
        var retrieved = await _unitOfWork.Movies.GetByIdAsync(movie.Id);
        
        Assert.NotNull(retrieved);
        Assert.Equal("Inception", retrieved.Title);
        Assert.Equal(user.Id, retrieved.UserId);
        Assert.Equal(MediaStatus.ToWatch, retrieved.Status);
    }
    
    [Fact]
    public async Task MovieRepository_GetUserMoviesWithPagination()
    {
        var user = new UserEntity 
        { 
            UserName = "testuser", 
            Email = "test@example.com" 
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        for (int i = 1; i <= 15; i++) // Create 15 movies
        {
            var movie = Movie.Create(
                user.Id,
                $"Movie {i}",
                TimeSpan.FromMinutes(120),
                new DateOnly(2024, 1, i)
            );
            await _unitOfWork.Movies.AddAsync(movie);
        }
        await _unitOfWork.SaveChangesAsync();
        
        var page1 = await _unitOfWork.Movies.GetMoviesAsync(
            user.Id,
            MovieOrderingCriteria.ByTitle,
            pageIndex: 0,
            countPerPage: 10);
        
        var page2 = await _unitOfWork.Movies.GetMoviesAsync(
            user.Id,
            MovieOrderingCriteria.ByTitle,
            pageIndex: 1,
            countPerPage: 10);
        
        Assert.Equal(15, page1.TotalCount);
        Assert.Equal(0, page1.PageIndex);
        Assert.Equal(10, page1.CountPerPage);
        Assert.Equal(10, page1.Items.Count());
        
        Assert.Equal(15, page2.TotalCount);
        Assert.Equal(1, page2.PageIndex);
        Assert.Equal(10, page2.CountPerPage);
        Assert.Equal(5, page2.Items.Count());
    }
    
    [Fact]
    public async Task MovieRepository_GetMoviesByStatus()
    {
        var user = new UserEntity 
        { 
            UserName = "testuser", 
            Email = "test@example.com" 
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var movie1 = Movie.Create(user.Id, "Movie 1", TimeSpan.FromMinutes(120), new DateOnly(2024, 1, 1));
        movie1.UpdateStatus(MediaStatus.Watched);
        
        var movie2 = Movie.Create(user.Id, "Movie 2", TimeSpan.FromMinutes(120), new DateOnly(2024, 1, 2));
        movie2.UpdateStatus(MediaStatus.Watching);
        
        var movie3 = Movie.Create(user.Id, "Movie 3", TimeSpan.FromMinutes(120), new DateOnly(2024, 1, 3));
        
        await _unitOfWork.Movies.AddAsync(movie1);
        await _unitOfWork.Movies.AddAsync(movie2);
        await _unitOfWork.Movies.AddAsync(movie3);
        await _unitOfWork.SaveChangesAsync();
        
        var watchedMovies = await _unitOfWork.Movies.GetMoviesByStatusAsync(
            user.Id,
            MediaStatus.Watched,
            pageIndex: 0,
            countPerPage: 10);
        
        var toWatchMovies = await _unitOfWork.Movies.GetMoviesByStatusAsync(
            user.Id,
            MediaStatus.ToWatch,
            pageIndex: 0,
            countPerPage: 10);
        
        Assert.Single(watchedMovies.Items);
        Assert.Equal("Movie 1", watchedMovies.Items.First().Title);
        
        Assert.Single(toWatchMovies.Items);
        Assert.Equal("Movie 3", toWatchMovies.Items.First().Title);
    }
    
    [Fact]
    public async Task MovieRepository_GetMoviesByTitle()
    {
        var user = new UserEntity 
        { 
            UserName = "testuser", 
            Email = "test@example.com" 
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        await _unitOfWork.Movies.AddAsync(Movie.Create(user.Id, "The Dark Knight", TimeSpan.FromMinutes(120), new DateOnly(2008, 1, 1)));
        await _unitOfWork.Movies.AddAsync(Movie.Create(user.Id, "The Dark Knight Rises", TimeSpan.FromMinutes(120), new DateOnly(2012, 1, 1)));
        await _unitOfWork.Movies.AddAsync(Movie.Create(user.Id, "Inception", TimeSpan.FromMinutes(120), new DateOnly(2010, 1, 1)));
        await _unitOfWork.SaveChangesAsync();
        
        var darkKnightMovies = await _unitOfWork.Movies.GetMoviesByTitleAsync(
            user.Id,
            "Dark Knight",
            pageIndex: 0,
            countPerPage: 10);
        
        Assert.Equal(2, darkKnightMovies.TotalCount);
        Assert.Equal(2, darkKnightMovies.Items.Count());
    }
    
    [Fact]
    public async Task MovieRepository_UpdateMovie()
    {
        var user = new UserEntity 
        { 
            UserName = "testuser", 
            Email = "test@example.com" 
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var movie = Movie.Create(user.Id, "Original Title", TimeSpan.FromMinutes(120), new DateOnly(2024, 1, 1));
        await _unitOfWork.Movies.AddAsync(movie);
        await _unitOfWork.SaveChangesAsync();
        
        movie.UpdateStatus(MediaStatus.Watched);
        movie.SetRating(9.5);
        _unitOfWork.Movies.Update(movie);
        await _unitOfWork.SaveChangesAsync();
        
        var updated = await _unitOfWork.Movies.GetByIdAsync(movie.Id);
        Assert.NotNull(updated);
        Assert.Equal(MediaStatus.Watched, updated.Status);
        Assert.Equal(9.5, updated.Rating);
    }
    
    [Fact]
    public async Task MovieRepository_DeleteMovie()
    {
        var user = new UserEntity 
        { 
            UserName = "testuser", 
            Email = "test@example.com" 
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var movie = Movie.Create(user.Id, "To Delete", TimeSpan.FromMinutes(120), new DateOnly(2024, 1, 1));
        await _unitOfWork.Movies.AddAsync(movie);
        await _unitOfWork.SaveChangesAsync();
        
        var movieId = movie.Id;
        
        _unitOfWork.Movies.Delete(movie);
        await _unitOfWork.SaveChangesAsync();
        
        var deleted = await _unitOfWork.Movies.GetByIdAsync(movieId);
        Assert.Null(deleted);
    }
    
    #endregion
    
    #region Serie Repository Tests
    
    [Fact]
    public async Task SerieRepository_CanAddAndRetrieveSerie()
    {
        var user = new UserEntity 
        { 
            UserName = "testuser", 
            Email = "test@example.com" 
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var serie = Serie.Create(
            user.Id,
            "Breaking Bad",
            new DateOnly(2008, 1, 20),
            "A high school chemistry teacher turned meth manufacturer"
        );
        
        await _unitOfWork.Series.AddAsync(serie);
        await _unitOfWork.SaveChangesAsync();
        
        var retrieved = await _unitOfWork.Series.GetByIdAsync(serie.Id);
        
        Assert.NotNull(retrieved);
        Assert.Equal("Breaking Bad", retrieved.Title);
        Assert.Equal(user.Id, retrieved.UserId);
    }
    
    [Fact]
    public async Task SerieRepository_GetUserSeriesWithPagination()
    {
        var user = new UserEntity 
        { 
            UserName = "testuser", 
            Email = "test@example.com" 
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        for (int i = 1; i <= 12; i++) // Create 12 series
        {
            var serie = Serie.Create(
                user.Id,
                $"Serie {i:D2}",
                new DateOnly(2024, 1, i)
            );
            await _unitOfWork.Series.AddAsync(serie);
        }
        await _unitOfWork.SaveChangesAsync();
        
        var page1 = await _unitOfWork.Series.GetSeriesAsync(
            user.Id,
            SerieOrderingCriteria.ByTitle,
            pageIndex: 0,
            countPerPage: 5);
        
        var page2 = await _unitOfWork.Series.GetSeriesAsync(
            user.Id,
            SerieOrderingCriteria.ByTitle,
            pageIndex: 1,
            countPerPage: 5);
        
        // Assert
        Assert.Equal(12, page1.TotalCount);
        Assert.Equal(5, page1.Items.Count());
        Assert.Equal(5, page2.Items.Count());
    }
    
    [Fact]
    public async Task SerieRepository_GetByIdWithSeasonsAndEpisodes()
    {
        var user = new UserEntity 
        { 
            UserName = "testuser", 
            Email = "test@example.com" 
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var serie = Serie.Create(user.Id, "Breaking Bad", new DateOnly(2008, 1, 20));
        
        var season1 = Season.Create(1, new DateOnly(2008, 1, 20), serie.Id);
        season1.AddEpisode(Episode.Create(1, "Pilot", TimeSpan.FromMinutes(58), season1.Id));
        season1.AddEpisode(Episode.Create(2, "Cat's in the Bag...", TimeSpan.FromMinutes(48), season1.Id));
        serie.AddSeason(season1);
        
        await _unitOfWork.Series.AddAsync(serie);
        await _unitOfWork.SaveChangesAsync();
        
        var retrieved = await _unitOfWork.Series.GetByIdWithSeasonsAndEpisodesAsync(serie.Id);
        
        Assert.NotNull(retrieved);
        Assert.Single(retrieved.Seasons);
        Assert.Equal(2, retrieved.Seasons.First().Episodes.Count);
    }
    
    #endregion
    
    #region Episode Repository Tests
    
    [Fact]
    public async Task EpisodeRepository_GetBySeasonIdWithPagination()
    {
        var user = new UserEntity 
        { 
            UserName = "testuser", 
            Email = "test@example.com" 
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var serie = Serie.Create(user.Id, "Test Serie", new DateOnly(2024, 1, 1));
        var season = Season.Create(1, new DateOnly(2024, 1, 1), serie.Id);
        
        for (int i = 1; i <= 15; i++) // Create 15 episodes
        {
            var episode = Episode.Create(i, $"Episode {i}", TimeSpan.FromMinutes(45), season.Id);
            season.AddEpisode(episode);
        }
        serie.AddSeason(season);
        
        await _unitOfWork.Series.AddAsync(serie);
        await _unitOfWork.SaveChangesAsync();
        
        var page1 = await _unitOfWork.Episodes.GetBySeasonIdAsync(
            season.Id,
            EpisodeOrderingCriteria.ByEpisodeNumber,
            pageIndex: 0,
            countPerPage: 10);
        
        Assert.Equal(15, page1.TotalCount);
        Assert.Equal(10, page1.Items.Count());
        Assert.Equal(1, page1.Items.First().EpisodeNumber);
    }
    
    [Fact]
    public async Task EpisodeRepository_GetWatchedByUser()
    {
        var user = new UserEntity 
        { 
            UserName = "testuser", 
            Email = "test@example.com" 
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var serie = Serie.Create(user.Id, "Test Serie", new DateOnly(2024, 1, 1));
        var season = Season.Create(1, new DateOnly(2024, 1, 1), serie.Id);
        
        var episode1 = Episode.Create(1, "Episode 1", TimeSpan.FromMinutes(45), season.Id);
        episode1.MarkAsWatched();
        
        var episode2 = Episode.Create(2, "Episode 2", TimeSpan.FromMinutes(45), season.Id);
        episode2.MarkAsWatched();
        
        var episode3 = Episode.Create(3, "Episode 3", TimeSpan.FromMinutes(45), season.Id);
        
        season.AddEpisode(episode1);
        season.AddEpisode(episode2);
        season.AddEpisode(episode3);
        serie.AddSeason(season);
        
        await _unitOfWork.Series.AddAsync(serie);
        await _unitOfWork.SaveChangesAsync();
        
        var watchedEpisodes = await _unitOfWork.Episodes.GetWatchedByUserAsync(
            user.Id,
            pageIndex: 0,
            countPerPage: 10);
        
        Assert.Equal(2, watchedEpisodes.TotalCount);
        Assert.Equal(2, watchedEpisodes.Items.Count());
        Assert.All(watchedEpisodes.Items, ep => Assert.True(ep.Watched));
    }
    
    [Fact]
    public async Task EpisodeRepository_GetNextUnwatchedEpisode()
    {
        var user = new UserEntity 
        { 
            UserName = "testuser", 
            Email = "test@example.com" 
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var serie = Serie.Create(user.Id, "Test Serie", new DateOnly(2024, 1, 1));
        var season1 = Season.Create(1, new DateOnly(2024, 1, 1), serie.Id);
        
        var episode1 = Episode.Create(1, "Episode 1", TimeSpan.FromMinutes(45), season1.Id);
        episode1.MarkAsWatched();
        
        var episode2 = Episode.Create(2, "Episode 2", TimeSpan.FromMinutes(45), season1.Id);
        
        var episode3 = Episode.Create(3, "Episode 3", TimeSpan.FromMinutes(45), season1.Id);
        
        season1.AddEpisode(episode1);
        season1.AddEpisode(episode2);
        season1.AddEpisode(episode3);
        serie.AddSeason(season1);
        
        await _unitOfWork.Series.AddAsync(serie);
        await _unitOfWork.SaveChangesAsync();
        
        var nextEpisode = await _unitOfWork.Episodes.GetNextUnwatchedEpisodeAsync(user.Id, serie.Id);
        
        Assert.NotNull(nextEpisode);
        Assert.Equal(2, nextEpisode.EpisodeNumber);
        Assert.Equal("Episode 2", nextEpisode.Title);
    }
    
    [Fact]
    public async Task EpisodeRepository_CountWatchedBySerieAsync()
    {
        var user = new UserEntity 
        { 
            UserName = "testuser", 
            Email = "test@example.com" 
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var serie = Serie.Create(user.Id, "Test Serie", new DateOnly(2024, 1, 1));
        var season = Season.Create(1, new DateOnly(2024, 1, 1), serie.Id);
        
        var episode1 = Episode.Create(1, "Episode 1", TimeSpan.FromMinutes(45), season.Id);
        episode1.MarkAsWatched();
        
        var episode2 = Episode.Create(2, "Episode 2", TimeSpan.FromMinutes(45), season.Id);
        episode2.MarkAsWatched();
        
        var episode3 = Episode.Create(3, "Episode 3", TimeSpan.FromMinutes(45), season.Id);
        
        season.AddEpisode(episode1);
        season.AddEpisode(episode2);
        season.AddEpisode(episode3);
        serie.AddSeason(season);
        
        await _unitOfWork.Series.AddAsync(serie);
        await _unitOfWork.SaveChangesAsync();
        
        var watchedCount = await _unitOfWork.Episodes.CountWatchedBySerieAsync(user.Id, serie.Id);
        var totalCount = await _unitOfWork.Episodes.CountBySerieAsync(user.Id, serie.Id);
        
        Assert.Equal(2, watchedCount);
        Assert.Equal(3, totalCount);
    }
    
    #endregion
    
    #region Season Repository Tests
    
    [Fact]
    public async Task SeasonRepository_GetBySerieId()
    {
        var user = new UserEntity 
        { 
            UserName = "testuser", 
            Email = "test@example.com" 
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var serie = Serie.Create(user.Id, "Test Serie", new DateOnly(2024, 1, 1));
        
        serie.AddSeason(Season.Create(1, new DateOnly(2024, 1, 1), serie.Id));
        serie.AddSeason(Season.Create(2, new DateOnly(2024, 2, 1), serie.Id));
        serie.AddSeason(Season.Create(3, new DateOnly(2024, 3, 1), serie.Id));
        
        await _unitOfWork.Series.AddAsync(serie);
        await _unitOfWork.SaveChangesAsync();
        
        var seasons = await _unitOfWork.Seasons.GetBySerieIdAsync(serie.Id);

        var enumerable = seasons as Season[] ?? seasons.ToArray();
        Assert.Equal(3, enumerable.Count());
        Assert.Equal(1, enumerable.First().SeasonNumber);
        Assert.Equal(3, enumerable.Last().SeasonNumber);
    }
    
    [Fact]
    public async Task SeasonRepository_GetSeasonProgress()
    {
        var user = new UserEntity 
        { 
            UserName = "testuser", 
            Email = "test@example.com" 
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var serie = Serie.Create(user.Id, "Test Serie", new DateOnly(2024, 1, 1));
        var season = Season.Create(1, new DateOnly(2024, 1, 1), serie.Id);
        
        var episode1 = Episode.Create(1, "Episode 1", TimeSpan.FromMinutes(45), season.Id);
        episode1.MarkAsWatched();
        
        var episode2 = Episode.Create(2, "Episode 2", TimeSpan.FromMinutes(45), season.Id);
        
        season.AddEpisode(episode1);
        season.AddEpisode(episode2);
        serie.AddSeason(season);
        
        await _unitOfWork.Series.AddAsync(serie);
        await _unitOfWork.SaveChangesAsync();
        
        var (total, watched) = await _unitOfWork.Seasons.GetSeasonProgressAsync(season.Id);
        
        Assert.Equal(2, total);
        Assert.Equal(1, watched);
    }
    
    #endregion

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}