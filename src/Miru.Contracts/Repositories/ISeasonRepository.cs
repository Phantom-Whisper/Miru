using Miru.Domain;
using Miru.Domain.Entities;

namespace Miru.Contracts.Repositories;

public interface ISeasonRepository: IRepository<Season>
{
    /// <summary>
    /// Retrieves all seasons associated with a specific series.
    /// </summary>
    /// <param name="serieId">
    /// The unique identifier of the series whose seasons should be retrieved.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A collection of <see cref="Season"/> belonging to the specified series.
    /// </returns>
    Task<IEnumerable<Season>> GetBySerieIdAsync(
        Guid serieId, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a season by its identifier, including its episodes.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the season.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// The <see cref="Season"/> with its episodes if found; otherwise, <c>null</c>.
    /// </returns>
    Task<Season?> GetByIdWithEpisodesAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a season by its series identifier and season number.
    /// </summary>
    /// <param name="serieId">
    /// The unique identifier of the series.
    /// </param>
    /// <param name="seasonNumber">
    /// The number of the season within the series.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// The matching <see cref="Season"/> if found; otherwise, <c>null</c>.
    /// </returns>
    Task<Season?> GetBySerieAndSeasonNumberAsync(
        Guid serieId,
        int seasonNumber,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves the watching progress of a specific season.
    /// </summary>
    /// <param name="seasonId">
    /// The unique identifier of the season.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A tuple containing:
    /// <list type="bullet">
    /// <item>
    /// <description><c>Total</c>: The total number of episodes in the season.</description>
    /// </item>
    /// <item>
    /// <description><c>Watched</c>: The number of episodes watched.</description>
    /// </item>
    /// </list>
    /// </returns>
    Task<(int Total, int Watched)> GetSeasonProgressAsync(
        Guid seasonId,
        CancellationToken cancellationToken = default);
}