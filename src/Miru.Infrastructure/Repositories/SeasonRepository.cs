using Microsoft.EntityFrameworkCore;
using Miru.Contracts.Repositories;
using Miru.Domain;

namespace Miru.Infrastructure.Repositories;

public class SeasonRepository : Repository<Season>, ISeasonRepository
{
    public SeasonRepository(MiruDbContext context) : base(context)
    {
    }
    
    /// <inheritdoc cref="ISeasonRepository.GetBySerieIdAsync"/>
    public async Task<IEnumerable<Season>> GetBySerieIdAsync(
        Guid serieId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(s => s.SerieId == serieId)
            .OrderBy(s => s.SeasonNumber)
            .ToListAsync(cancellationToken);
    }
    
    /// <inheritdoc cref="ISeasonRepository.GetByIdWithEpisodesAsync"/>
    public async Task<Season?> GetByIdWithEpisodesAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Episodes)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
    
    /// <inheritdoc cref="ISeasonRepository.GetBySerieAndSeasonNumberAsync"/>
    public async Task<Season?> GetBySerieAndSeasonNumberAsync(
        Guid serieId,
        int seasonNumber,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Episodes)
            .FirstOrDefaultAsync(s => s.SerieId == serieId && s.SeasonNumber == seasonNumber, cancellationToken);
    }
    
    /// <inheritdoc cref="ISeasonRepository.GetSeasonProgressAsync"/>
    public async Task<(int Total, int Watched)> GetSeasonProgressAsync(
        Guid seasonId,
        CancellationToken cancellationToken = default)
    {
        var season = await DbSet
            .Include(s => s.Episodes)
            .FirstOrDefaultAsync(s => s.Id == seasonId, cancellationToken);
        
        if (season == null)
            return (0, 0);
        
        var total = season.Episodes.Count;
        var watched = season.Episodes.Count(e => e.Watched);
        
        return (total, watched);
    }
}