using Microsoft.EntityFrameworkCore;
using Miru.Domain;
using Miru.Domain.Exceptions;
using Miru.Infrastructure;

namespace Miru.Test.Infrastructure;

public class MiruDbContextTests : IDisposable
{
    private readonly MiruDbContext _context;

    public MiruDbContextTests()
    {
        var databaseName = $"MiruTestDb_{Guid.NewGuid()}";
        var connectionString = $"Server=(localdb)\\mssqllocaldb;Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=true";
        
        var options = new DbContextOptionsBuilder<MiruDbContext>()
            .UseSqlServer(connectionString)
            .EnableSensitiveDataLogging()
            .Options;
        
        _context = new MiruDbContext(options);
        _context.Database.EnsureCreated();
    }
    
    [Fact]
    public async Task CanCreateAndRetrieveMovie()
    {
        var user = new UserEntity 
        { 
            UserName = "testuser", 
            Email = "test@example.com" 
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var movie = Movie.Create(
            user.Id, // userId required
            "Inception",
            TimeSpan.FromMinutes(148),
            new DateOnly(2010, 7, 16),
            "A thief who steals corporate secrets through dreams",
            "https://example.com/inception.jpg"
        );
        
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();
        
        var retrieved = await _context.Movies.FirstOrDefaultAsync(m => m.Id == movie.Id);
        
        Assert.NotNull(retrieved);
        Assert.Equal("Inception", retrieved.Title);
        Assert.Equal(TimeSpan.FromMinutes(148), retrieved.Duration);
        Assert.Equal(new DateOnly(2010, 7, 16), retrieved.ReleaseDate);
        Assert.Equal(user.Id, retrieved.UserId); // Verify ownership
        Assert.Equal(MediaStatus.ToWatch, retrieved.Status); // Default status
    }
    
    [Fact]
    public async Task CanCreateSerieWithSeasonsAndEpisodes()
    {
        var user = new UserEntity 
        { 
            UserName = "testuser", 
            Email = "test@example.com" 
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var serie = Serie.Create(
            user.Id, // userId required
            "Breaking Bad",
            new DateOnly(2008, 1, 20),
            "A high school chemistry teacher turned meth manufacturer",
            "https://example.com/breaking-bad.jpg"
        );
        
        var season1 = Season.Create(1, new DateOnly(2008, 1, 20), serie.Id);
        var episode1 = Episode.Create(1, "Pilot", TimeSpan.FromMinutes(58), season1.Id);
        var episode2 = Episode.Create(2, "Cat's in the Bag...", TimeSpan.FromMinutes(48), season1.Id);
        
        season1.AddEpisode(episode1);
        season1.AddEpisode(episode2);
        serie.AddSeason(season1);
        
        var season2 = Season.Create(2, new DateOnly(2009, 3, 8), serie.Id);
        var episode3 = Episode.Create(1, "Seven Thirty-Seven", TimeSpan.FromMinutes(47), season2.Id);
        
        season2.AddEpisode(episode3);
        serie.AddSeason(season2);
        
        _context.Series.Add(serie);
        await _context.SaveChangesAsync();
        
        var retrieved = await _context.Series
            .Include(s => s.Seasons)
            .ThenInclude(season => season.Episodes)
            .FirstOrDefaultAsync(s => s.Id == serie.Id);
        
        Assert.NotNull(retrieved);
        Assert.Equal("Breaking Bad", retrieved.Title);
        Assert.Equal(user.Id, retrieved.UserId); // Verify ownership
        Assert.Equal(2, retrieved.Seasons.Count);
        Assert.Equal(2, retrieved.Seasons.First(s => s.SeasonNumber == 1).Episodes.Count);
        Assert.Single(retrieved.Seasons.First(s => s.SeasonNumber == 2).Episodes);
    }
    
    [Fact]
    public async Task MediaDiscriminatorWorksCorrectly()
    {
        var user = new UserEntity 
        { 
            UserName = "testuser", 
            Email = "test@example.com" 
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var movie = Movie.Create(user.Id, "Test Movie", TimeSpan.FromMinutes(120), new DateOnly(2024, 1, 1));
        var serie = Serie.Create(user.Id, "Test Serie", new DateOnly(2024, 1, 1));
        
        _context.Movies.Add(movie);
        _context.Series.Add(serie);
        await _context.SaveChangesAsync();
        
        var allMedia = await _context.Set<Media>().ToListAsync();
        var movies = allMedia.OfType<Movie>().ToList();
        var series = allMedia.OfType<Serie>().ToList();
        
        Assert.Equal(2, allMedia.Count);
        Assert.Single(movies);
        Assert.Single(series);
    }
    
    [Fact]
    public async Task SeasonNumbersAreUniquePerSerie()
    {
        // Arrange - Create user first
        var user = new UserEntity 
        { 
            UserName = "testuser", 
            Email = "test@example.com" 
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    
        var serie = Serie.Create(user.Id, "Test Serie", new DateOnly(2024, 1, 1));
        var season1 = Season.Create(1, new DateOnly(2024, 1, 1), serie.Id);
        var season1Duplicate = Season.Create(1, new DateOnly(2024, 2, 1), serie.Id);
    
        serie.AddSeason(season1);
    
        // Act & Assert - Exception thrown by domain logic, not EF
        var exception = Assert.Throws<DomainException>(() => 
            serie.AddSeason(season1Duplicate));
    
        Assert.Equal("Season 1 already exists", exception.Message);
    }
    
    [Fact]
    public async Task EpisodeNumbersAreUniquePerSeason()
    {
        // Arrange - Create user first
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
        var episode1Duplicate = Episode.Create(1, "Episode 1 Duplicate", TimeSpan.FromMinutes(45), season.Id);
    
        season.AddEpisode(episode1);
    
        // Act & Assert - Exception thrown by domain logic, not EF
        var exception = Assert.Throws<DomainException>(() => 
            season.AddEpisode(episode1Duplicate));
    
        Assert.Equal("Episode 1 already exists in this season", exception.Message);
    }
    
    [Fact]
    public async Task CascadeDeleteWorksForSerieSeasonEpisode()
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
        var episode = Episode.Create(1, "Pilot", TimeSpan.FromMinutes(45), season.Id);
        
        season.AddEpisode(episode);
        serie.AddSeason(season);
        
        _context.Series.Add(serie);
        await _context.SaveChangesAsync();
        
        var serieId = serie.Id;
        
        _context.Series.Remove(serie);
        await _context.SaveChangesAsync();
        
        var remainingSeasons = await _context.Seasons.Where(s => s.SerieId == serieId).ToListAsync();
        var remainingEpisodes = await _context.Episodes.Where(e => e.Season.SerieId == serieId).ToListAsync();
        
        Assert.Empty(remainingSeasons);
        Assert.Empty(remainingEpisodes);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}