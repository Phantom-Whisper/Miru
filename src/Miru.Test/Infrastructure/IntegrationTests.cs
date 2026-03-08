using Microsoft.EntityFrameworkCore;
using Miru.Domain;
using Miru.Domain.Entities;
using Miru.Infrastructure;
using Miru.Shared.Common.Enums;

namespace Miru.Test.Infrastructure;

public class IntegrationTests : IDisposable
{
    private readonly MiruDbContext _context;

    public IntegrationTests()
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
    public async Task CompleteWorkflow_CreateSerieAndTrackProgress()
    {
        // Create user
        var user = new UserEntity 
        { 
            UserName = "john.doe", 
            Email = "john@example.com" 
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        // User creates a serie in their library
        var serie = Serie.Create(
            user.Id,
            "Game of Thrones",
            new DateOnly(2011, 4, 17),
            "Nine noble families fight for control over the lands of Westeros"
        );
        
        var season1 = Season.Create(1, new DateOnly(2011, 4, 17), serie.Id);
        var episode1 = Episode.Create(1, "Winter Is Coming", TimeSpan.FromMinutes(62), season1.Id);
        var episode2 = Episode.Create(2, "The Kingsroad", TimeSpan.FromMinutes(56), season1.Id);
        
        season1.AddEpisode(episode1);
        season1.AddEpisode(episode2);
        serie.AddSeason(season1);
        
        _context.Series.Add(serie);
        await _context.SaveChangesAsync();
        
        // User marks episodes as watched and rates them
        episode1.MarkAsWatched();
        episode1.SetRating(9.0);
        
        episode2.MarkAsWatched();
        episode2.SetRating(8.5);
        
        await _context.SaveChangesAsync();
        
        // User updates serie status
        serie.UpdateStatus(MediaStatus.Watching);
        serie.SetRating(9.5);
        await _context.SaveChangesAsync();
        
        // Verify the complete workflow
        var userLibrary = await _context.Series
            .Include(s => s.Seasons)
                .ThenInclude(s => s.Episodes)
            .Where(s => s.UserId == user.Id)
            .ToListAsync();
        
        Assert.Single(userLibrary);
        
        var retrievedSerie = userLibrary.First();
        Assert.Equal("Game of Thrones", retrievedSerie.Title);
        Assert.Equal(MediaStatus.Watching, retrievedSerie.Status); // ← Serie status
        Assert.Equal(9.5, retrievedSerie.Rating); // ← Serie rating
        Assert.Single(retrievedSerie.Seasons);
        
        var retrievedSeason = retrievedSerie.Seasons.First();
        Assert.Equal(2, retrievedSeason.Episodes.Count);
        
        // Verify episode tracking
        Assert.All(retrievedSeason.Episodes, ep => 
        {
            Assert.True(ep.Watched); // All episodes watched
            Assert.NotNull(ep.WatchedAt);
            Assert.NotNull(ep.Rating);
        });
    }
    
    [Fact]
    public async Task MultipleUsers_CanHaveSeparateLibraries()
    {
        // Create two users
        var user1 = new UserEntity 
        { 
            UserName = "user1", 
            Email = "user1@example.com" 
        };
        var user2 = new UserEntity 
        { 
            UserName = "user2", 
            Email = "user2@example.com" 
        };
        
        _context.Users.AddRange(user1, user2);
        await _context.SaveChangesAsync();
        
        // Each user creates their own "Breaking Bad" (separate entries)
        var user1Serie = Serie.Create(user1.Id, "Breaking Bad", new DateOnly(2008, 1, 20));
        var user2Serie = Serie.Create(user2.Id, "Breaking Bad", new DateOnly(2008, 1, 20));
        
        _context.Series.AddRange(user1Serie, user2Serie);
        await _context.SaveChangesAsync();
        
        // User1 marks their serie as watched
        user1Serie.UpdateStatus(MediaStatus.Watched);
        user1Serie.SetRating(10.0);
        await _context.SaveChangesAsync();
        
        // Verify libraries are separate
        var user1Library = await _context.Series.Where(s => s.UserId == user1.Id).ToListAsync();
        var user2Library = await _context.Series.Where(s => s.UserId == user2.Id).ToListAsync();
        
        Assert.Single(user1Library);
        Assert.Single(user2Library);
        
        // User 1's serie is watched
        Assert.Equal(MediaStatus.Watched, user1Library[0].Status);
        Assert.Equal(10.0, user1Library[0].Rating);
        
        // User 2's serie is still to watch
        Assert.Equal(MediaStatus.ToWatch, user2Library[0].Status);
        Assert.Null(user2Library[0].Rating);
    }
    
    [Fact]
    public async Task UserCanTrackMoviesAndSeries()
    {
        // Create user
        var user = new UserEntity 
        { 
            UserName = "moviefan", 
            Email = "fan@example.com" 
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        // User adds movies
        var inception = Movie.Create(
            user.Id, 
            "Inception", 
            TimeSpan.FromMinutes(148), 
            new DateOnly(2010, 7, 16)
        );
        inception.UpdateStatus(MediaStatus.Watched);
        inception.SetRating(9.0);
        
        var interstellar = Movie.Create(
            user.Id, 
            "Interstellar", 
            TimeSpan.FromMinutes(169), 
            new DateOnly(2014, 11, 7)
        );
        // Left as ToWatch
        
        _context.Movies.AddRange(inception, interstellar);
        
        // User adds a serie
        var breakingBad = Serie.Create(
            user.Id,
            "Breaking Bad",
            new DateOnly(2008, 1, 20)
        );
        breakingBad.UpdateStatus(MediaStatus.Watching);
        
        _context.Series.Add(breakingBad);
        await _context.SaveChangesAsync();
        
        // Verify user's complete library
        var allMedia = await _context.Set<Media>()
            .Where(m => m.UserId == user.Id)
            .ToListAsync();
        
        Assert.Equal(3, allMedia.Count);
        
        var watchedMedia = allMedia.Where(m => m.Status == MediaStatus.Watched).ToList();
        var toWatchMedia = allMedia.Where(m => m.Status == MediaStatus.ToWatch).ToList();
        var watchingMedia = allMedia.Where(m => m.Status == MediaStatus.Watching).ToList();
        
        Assert.Single(watchedMedia); // Inception
        Assert.Single(toWatchMedia); // Interstellar
        Assert.Single(watchingMedia); // Breaking Bad
    }
    
    [Fact]
    public async Task DeletingUser_DeletesAllTheirMedia()
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
        
        // Act
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        
        // Assert
        var remainingMedia = await _context.Set<Media>().Where(m => m.UserId == user.Id).ToListAsync();
        Assert.Empty(remainingMedia);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}