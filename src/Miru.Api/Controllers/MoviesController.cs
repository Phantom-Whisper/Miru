using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Miru.Application.Interfaces;
using Miru.Shared.Common;
using Miru.Shared.DTOs.Movies;
using Miru.Domain;
using Miru.Shared.Common.Enums;

namespace Miru.Api.Controllers;

/// <summary>
/// Controller managing movies.
/// </summary>
/// <param name="service"></param>
/// <param name="logger"></param>
[ApiController]
[Route("movies")]
[Authorize]
public class MoviesController(IMovieService service, ILogger<MoviesController> logger) : ControllerBase
{
    /// <summary>
    /// Get all my movies with pagination and sorting.
    /// </summary>
    /// <param name="orderingCriteria">Sorting criteria (None, ByTitle, ByReleaseDate, etc.)</param>
    /// <param name="pageIndex">Page index (0-based)</param>
    /// <param name="countPerPage">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of movies</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagingResult<MovieDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagingResult<MovieDto>>> GetMoviesAsync(
        [FromQuery] MovieOrderingCriteria orderingCriteria = MovieOrderingCriteria.None,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await service.GetMoviesAsync(
            orderingCriteria,
            pageIndex,
            countPerPage,
            cancellationToken);
        
        return Ok(result);
    }

    /// <summary>
    /// Get a specific movie by ID.
    /// </summary>
    /// <param name="id">Movie ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Movie details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MovieDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieDto>> GetMovieByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var result = await service.GetMovieByIdAsync(id, cancellationToken);
        
        if (result == null)
        {
            logger.LogWarning("Movie with ID: {MovieId} not found", id);
            return NotFound();
        }
        
        return Ok(result);
    }

    /// <summary>
    /// Get movies by status (ToWatch, Watching, Watched).
    /// </summary>
    /// <param name="status">Status filter (ToWatch, Watching, or Watched)</param>
    /// <param name="orderingCriteria">Sorting criteria</param>
    /// <param name="pageIndex">Page index (0-based)</param>
    /// <param name="countPerPage">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of movies with the specified status</returns>
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(PagingResult<MovieDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagingResult<MovieDto>>> GetMoviesByStatusAsync(
        MediaStatus status,
        [FromQuery] MovieOrderingCriteria orderingCriteria = MovieOrderingCriteria.None,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await service.GetMoviesByStatusAsync(
            status,
            orderingCriteria,
            pageIndex,
            countPerPage,
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Search movies by title.
    /// </summary>
    /// <param name="title">Search term for movie title</param>
    /// <param name="orderBy">Sorting criteria</param>
    /// <param name="pageIndex">Page index (0-based)</param>
    /// <param name="countPerPage">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of movies matching the search criteria</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagingResult<MovieDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagingResult<MovieDto>>> SearchMoviesAsync(
        [FromQuery] string title,
        [FromQuery] MovieOrderingCriteria orderBy = MovieOrderingCriteria.None,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await service.SearchMoviesByTitleAsync(
            title,
            orderBy,
            pageIndex,
            countPerPage,
            cancellationToken);
        
        return Ok(result);
    }

    /// <summary>
    /// Create a new movie.
    /// </summary>
    /// <param name="createMovieDto">Movie creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created movie details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(MovieDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<MovieDetailsDto>> AddMovieAsync(
        [FromBody] CreateMovieDto createMovieDto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state for movie creation");
            return BadRequest(ModelState);
        }
        
        var movie = await service.CreateMovieAsync(createMovieDto, cancellationToken);
        
        return Created($"/api/movies/{movie.Id}", movie);
    }

    /// <summary>
    /// Update an existing movie.
    /// </summary>
    /// <param name="id">Movie ID</param>
    /// <param name="updateMovieDto">Movie update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated movie details</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(MovieDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieDetailsDto>> UpdateMovieAsync(
        Guid id,
        [FromBody] UpdateMovieDto updateMovieDto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var movie = await service.UpdateMovieAsync(id, updateMovieDto, cancellationToken);
        
        return Ok(movie);
    }

    /// <summary>
    /// Update movie status (ToWatch, Watching, Watched).
    /// </summary>
    /// <param name="id">Movie ID</param>
    /// <param name="dto">Status update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMovieStatusAsync(
        Guid id,
        [FromBody] UpdateMovieStatusDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        await service.UpdateMovieStatusAsync(id, dto.Status, cancellationToken);
        
        return NoContent();
    }

    /// <summary>
    /// Rate a movie (0-10).
    /// </summary>
    /// <param name="id">Movie ID</param>
    /// <param name="dto">Rating data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}/rating")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMovieRatingAsync(
        Guid id,
        [FromBody] RateMovieDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        await service.RateMovieAsync(id, dto.Rating, cancellationToken);
        
        return NoContent();
    }

    /// <summary>
    /// Delete a movie.
    /// </summary>
    /// <param name="id">Movie ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMovieAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await service.DeleteMovieAsync(id, cancellationToken);
        
        return NoContent();
    }
}