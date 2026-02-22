namespace Miru.Contracts.DTOs.Movies;

/// <summary>
/// Complete movie information including description and dates.
/// </summary>
public class MovieDetailsDto : MovieDto
{
    /// <summary>
    /// Description of the movie.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Release date of the movie.
    /// </summary>
    public DateOnly ReleaseDate { get; set; }
    
    /// <summary>
    /// Date the movie was added by the user.
    /// </summary>
    public DateTime AddedAt { get; set; }
    
    /// <summary>
    /// Date the movie was watched by the user.
    /// </summary>
    public DateTime? WatchedAt { get; set; }
}