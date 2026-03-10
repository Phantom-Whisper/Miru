using Miru.Shared.Common.Enums;

namespace Miru.Web.Models;

public class MovieModel
{
    /// <summary>
    /// Unique identifier for the movie.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Title of the movie.
    /// </summary>
    public string Title { get; set; } = null!;
    
    /// <summary>
    /// Poster URL of the movie.
    /// </summary>
    public string? PosterUrl { get; set; }
    
    /// <summary>
    /// Current status of the movie.
    /// </summary>
    public MediaStatus Status { get; set; }
    
    /// <summary>
    /// Rating given by the user to the movie.
    /// </summary>
    public double? Rating { get; set; }
    
    /// <summary>
    /// Duration of the movie in minutes.
    /// </summary>
    public int Duration { get; set; }
}