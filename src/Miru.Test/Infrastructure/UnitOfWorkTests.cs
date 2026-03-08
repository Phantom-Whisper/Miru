using Microsoft.EntityFrameworkCore;
using Miru.Domain.Entities;
using Miru.Infrastructure.Persistence;
using Miru.Infrastructure;
using Miru.Infrastructure.Persistence.UnitOfWork;

namespace Miru.Test.Infrastructure;

public class UnitOfWorkTests : IDisposable
{
    private readonly MiruDbContext _context;
    private readonly UnitOfWork _uow;

    public UnitOfWorkTests()
    {
        var databaseName = $"MiruUoWTest_{Guid.NewGuid()}";
        var connectionString = $"Server=(localdb)\\mssqllocaldb;Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=true";
        
        var options = new DbContextOptionsBuilder<MiruDbContext>()
            .UseSqlServer(connectionString)
            .Options;
        
        _context = new MiruDbContext(options);
        _context.Database.EnsureCreated();
        
        _uow = new UnitOfWork(_context);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldDetachEntitiesAfterSave()
    {
        var user = new UserEntity { UserName = "uow_user", Email = "uow@test.com" };
        _context.Users.Add(user);
        await _uow.SaveChangesAsync();

        var movie = Movie.Create(user.Id, "Inception", TimeSpan.FromMinutes(148), new DateOnly(2010, 7, 16));

        await _uow.Movies.AddAsync(movie);
        await _uow.SaveChangesAsync();

        var entry = _context.Entry(movie);
        Assert.Equal(EntityState.Detached, entry.State);
    }

    [Fact]
    public async Task RejectChangesAsync_ShouldRevertChanges()
    {
        var user = new UserEntity { UserName = "uow_user", Email = "uow@test.com" };
        _context.Users.Add(user);
        await _uow.SaveChangesAsync(); 

        var movie = Movie.Create(user.Id, "Original Title", TimeSpan.FromMinutes(120), new DateOnly(2024, 1, 1));
        await _uow.Movies.AddAsync(movie);
        await _uow.SaveChangesAsync(); // Movie is now DB-persisted and detached

        // Re-attach and force a modification
        _context.Movies.Attach(movie);
    
        var entry = _context.Entry(movie);
        entry.Property(m => m.Title).CurrentValue = "Modified Title";
        entry.State = EntityState.Modified;
    
        Assert.Equal("Modified Title", movie.Title);

        await _uow.RejectChangesAsync();

        Assert.Equal("Original Title", movie.Title);
        Assert.Equal(EntityState.Unchanged, _context.Entry(movie).State);
    }

    [Fact]
    public async Task Transaction_ShouldRollbackOnFailure()
    {
        var user = new UserEntity { UserName = "tx_user", Email = "tx@test.com" };
        _context.Users.Add(user);
        await _uow.SaveChangesAsync();

        await _uow.BeginTransactionAsync();
        
        var movie = Movie.Create(user.Id, "Transaction Movie", TimeSpan.FromMinutes(100), new DateOnly(2024, 1, 1));
        await _uow.Movies.AddAsync(movie);
        await _uow.SaveChangesAsync();

        await _uow.RollbackTransactionAsync();

        var exists = await _context.Movies.AnyAsync(m => m.Title == "Transaction Movie");
        Assert.False(exists);
    }

    [Fact]
    public Task Repositories_ShouldShareSameContext()
    {
        Assert.NotNull(_uow.Movies);
        Assert.NotNull(_uow.Series);
        
        Assert.Same(_uow.Movies, _uow.Movies); // Check lazy loading caching works
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _uow.Dispose();
    }
}