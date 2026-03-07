using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Miru.Contracts.Common;
using Miru.Contracts.DTOs.Episodes;
using Miru.Contracts.DTOs.Seasons;
using Miru.Contracts.DTOs.Series;
using Miru.Contracts.Services;
using Miru.Domain;

namespace Miru.Api.Controllers;

[ApiController]
[Route("series")]
[Authorize]
public class SeriesController(
    ISerieService serieService,
    ISeasonService seasonService,
    IEpisodeService episodeService,
    ILogger<SeriesController> logger)
    : ControllerBase
{
    /// <summary>
    /// Get all my series with pagination and sorting.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagingResult<SerieDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagingResult<SerieDto>>> GetSeriesAsync(
        [FromQuery] SerieOrderingCriteria orderBy = SerieOrderingCriteria.None,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching user's series with criteria: {Criteria}, page: {Page}", 
            orderBy, pageIndex);
        
        var result = await serieService.GetSeriesAsync(orderBy, pageIndex, countPerPage, cancellationToken);
        
        logger.LogInformation("Successfully fetched {Count} series out of {Total}", 
            result.Items.Count(), result.TotalCount);
        
        return Ok(result);
    }

    /// <summary>
    /// Get a specific serie with all details (seasons and episodes).
    /// </summary>
    [HttpGet("{serieId}")]
    [ProducesResponseType(typeof(SerieDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SerieDetailsDto>> GetSerieAsync(
        Guid serieId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching serie with ID: {SerieId}", serieId);
        
        var result = await serieService.GetSerieByIdAsync(serieId, cancellationToken);
        
        if (result == null)
        {
            logger.LogWarning("Serie with ID: {SerieId} not found", serieId);
            return NotFound();
        }
        
        logger.LogInformation("Successfully fetched serie: {Title}", result.Title);
        return Ok(result);
    }

    /// <summary>
    /// Get series by status (ToWatch, Watching, Watched).
    /// </summary>
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(PagingResult<SerieDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagingResult<SerieDto>>> GetSeriesByStatusAsync(
        MediaStatus status,
        [FromQuery] SerieOrderingCriteria orderBy = SerieOrderingCriteria.None,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching series with status: {Status}", status);
        
        var result = await serieService.GetSeriesByStatusAsync(
            status,
            orderBy,
            pageIndex,
            countPerPage,
            cancellationToken);

        logger.LogInformation("Successfully fetched {Count} series with status: {Status}", 
            result.Items.Count(), status);
        
        return Ok(result);
    }

    /// <summary>
    /// Create a new serie.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SerieDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SerieDetailsDto>> CreateSerieAsync(
        [FromBody] CreateSerieDto createSerieDto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state for serie creation");
            return BadRequest(ModelState);
        }
        
        logger.LogInformation("Creating new serie: {Title}", createSerieDto.Title);
        
        var serie = await serieService.CreateSerieAsync(createSerieDto, cancellationToken);
        
        logger.LogInformation("Successfully created serie with ID: {SerieId}", serie.Id);
        
        return Created($"/api/series/{serie.Id}", serie);
    }

    /// <summary>
    /// Update an existing serie.
    /// </summary>
    [HttpPut("{serieId}")]
    [ProducesResponseType(typeof(SerieDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SerieDetailsDto>> UpdateSerieAsync(
        Guid serieId,
        [FromBody] UpdateSerieDto updateSerieDto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state for serie update: {SerieId}", serieId);
            return BadRequest(ModelState);
        }
        
        logger.LogInformation("Updating serie with ID: {SerieId}", serieId);
        
        var serie = await serieService.UpdateSerieAsync(serieId, updateSerieDto, cancellationToken);
        
        logger.LogInformation("Successfully updated serie: {SerieId}", serieId);
        
        return Ok(serie);
    }

    /// <summary>
    /// Update serie status (ToWatch, Watching, Watched).
    /// </summary>
    [HttpPut("{serieId}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSerieStatusAsync(
        Guid serieId,
        [FromBody] UpdateSerieStatusDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state for status update: {SerieId}", serieId);
            return BadRequest(ModelState);
        }
        
        logger.LogInformation("Updating status for serie {SerieId} to {Status}", serieId, dto.Status);
        
        await serieService.UpdateSerieStatusAsync(serieId, dto.Status.ToString(), cancellationToken);
        
        logger.LogInformation("Successfully updated status for serie: {SerieId}", serieId);
        
        return NoContent();
    }

    /// <summary>
    /// Rate a serie (0-10).
    /// </summary>
    [HttpPut("{serieId}/rating")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RateSerieAsync(
        Guid serieId,
        [FromBody] RateSerieDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state for rating update: {SerieId}", serieId);
            return BadRequest(ModelState);
        }
        
        logger.LogInformation("Rating serie {SerieId} with {Rating}", serieId, dto.Rating);
        
        await serieService.RateSerieAsync(serieId, dto.Rating, cancellationToken);
        
        logger.LogInformation("Successfully rated serie: {SerieId}", serieId);
        
        return NoContent();
    }

    /// <summary>
    /// Delete a serie.
    /// </summary>
    [HttpDelete("{serieId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSerieAsync(
        Guid serieId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting serie with ID: {SerieId}", serieId);
        
        await serieService.DeleteSerieAsync(serieId, cancellationToken);
        
        logger.LogInformation("Successfully deleted serie: {SerieId}", serieId);
        
        return NoContent();
    }

    // ==================== SEASONS ====================

    /// <summary>
    /// Get all seasons of a serie.
    /// </summary>
    [HttpGet("{serieId}/seasons")]
    [ProducesResponseType(typeof(IEnumerable<SeasonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<SeasonDto>>> GetSeasonsAsync(
        Guid serieId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching seasons for serie: {SerieId}", serieId);
        
        var seasons = await seasonService.GetSeasonsBySerieIdAsync(serieId, cancellationToken);
        
        logger.LogInformation("Successfully fetched {Count} seasons for serie: {SerieId}", 
            seasons.Count(), serieId);
        
        return Ok(seasons);
    }

    /// <summary>
    /// Get a specific season with all episodes.
    /// </summary>
    [HttpGet("{serieId}/seasons/{seasonId}")]
    [ProducesResponseType(typeof(SeasonDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SeasonDetailsDto>> GetSeasonAsync(
        Guid serieId,
        Guid seasonId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching season {SeasonId} for serie {SerieId}", seasonId, serieId);
        
        var season = await seasonService.GetSeasonByIdAsync(serieId, seasonId, cancellationToken);
        
        if (season == null)
        {
            logger.LogWarning("Season {SeasonId} not found for serie {SerieId}", seasonId, serieId);
            return NotFound();
        }
        
        logger.LogInformation("Successfully fetched season {SeasonId}", seasonId);
        return Ok(season);
    }

    /// <summary>
    /// Add a season to a serie.
    /// </summary>
    [HttpPost("{serieId}/seasons")]
    [ProducesResponseType(typeof(SeasonDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SeasonDetailsDto>> CreateSeasonAsync(
        Guid serieId,
        [FromBody] CreateSeasonDto createSeasonDto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state for season creation");
            return BadRequest(ModelState);
        }
        
        logger.LogInformation("Creating season {SeasonNumber} for serie {SerieId}", 
            createSeasonDto.SeasonNumber, serieId);
        
        var season = await seasonService.AddSeasonToSerieAsync(serieId, createSeasonDto, cancellationToken);
        
        logger.LogInformation("Successfully created season {SeasonId} for serie {SerieId}", 
            season.Id, serieId);
        
        return Created($"/api/series/{serieId}/seasons/{season.Id}", season);
    }

    /// <summary>
    /// Update a season.
    /// </summary>
    [HttpPut("{serieId}/seasons/{seasonId}")]
    [ProducesResponseType(typeof(SeasonDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SeasonDetailsDto>> UpdateSeasonAsync(
        Guid serieId,
        Guid seasonId,
        [FromBody] UpdateSeasonDto updateSeasonDto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state for season update: {SeasonId}", seasonId);
            return BadRequest(ModelState);
        }
        
        logger.LogInformation("Updating season {SeasonId} for serie {SerieId}", seasonId, serieId);
        
        var season = await seasonService.UpdateSeasonAsync(serieId, seasonId, updateSeasonDto, cancellationToken);
        
        logger.LogInformation("Successfully updated season {SeasonId}", seasonId);
        
        return Ok(season);
    }

    /// <summary>
    /// Mark all episodes of a season as watched.
    /// </summary>
    [HttpPut("{serieId}/seasons/{seasonId}/watch")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkSeasonAsWatchedAsync(
        Guid serieId,
        Guid seasonId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Marking all episodes as watched for season {SeasonId}", seasonId);
        
        await seasonService.MarkSeasonAsWatchedAsync(serieId, seasonId, cancellationToken);
        
        logger.LogInformation("Successfully marked season {SeasonId} as watched", seasonId);
        
        return NoContent();
    }

    /// <summary>
    /// Delete a season.
    /// </summary>
    [HttpDelete("{serieId}/seasons/{seasonId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSeasonAsync(
        Guid serieId,
        Guid seasonId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting season {SeasonId} from serie {SerieId}", seasonId, serieId);
        
        await seasonService.DeleteSeasonAsync(serieId, seasonId, cancellationToken);
        
        logger.LogInformation("Successfully deleted season {SeasonId}", seasonId);
        
        return NoContent();
    }

    // ==================== EPISODES ====================

    /// <summary>
    /// Get all episodes of a season.
    /// </summary>
    [HttpGet("{serieId}/seasons/{seasonId}/episodes")]
    [ProducesResponseType(typeof(PagingResult<EpisodeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagingResult<EpisodeDto>>> GetEpisodesAsync(
        Guid serieId,
        Guid seasonId,
        [FromQuery] EpisodeOrderingCriteria orderBy = EpisodeOrderingCriteria.ByEpisodeNumber,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int countPerPage = 50,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching episodes for season {SeasonId} of serie {SerieId}", 
            seasonId, serieId);
        
        var result = await episodeService.GetEpisodesBySeasonIdAsync(
            serieId, seasonId, orderBy, pageIndex, countPerPage, cancellationToken);
        
        logger.LogInformation("Successfully fetched {Count} episodes", result.Items.Count());
        
        return Ok(result);
    }

    /// <summary>
    /// Get a specific episode.
    /// </summary>
    [HttpGet("{serieId}/seasons/{seasonId}/episodes/{episodeId}")]
    [ProducesResponseType(typeof(EpisodeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EpisodeDto>> GetEpisodeAsync(
        Guid serieId,
        Guid seasonId,
        Guid episodeId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching episode {EpisodeId}", episodeId);
        
        var episode = await episodeService.GetEpisodeByIdAsync(
            serieId, seasonId, episodeId, cancellationToken);
        
        if (episode == null)
        {
            logger.LogWarning("Episode {EpisodeId} not found", episodeId);
            return NotFound();
        }
        
        logger.LogInformation("Successfully fetched episode: {Title}", episode.Title);
        return Ok(episode);
    }

    /// <summary>
    /// Add an episode to a season.
    /// </summary>
    [HttpPost("{serieId}/seasons/{seasonId}/episodes")]
    [ProducesResponseType(typeof(EpisodeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EpisodeDto>> CreateEpisodeAsync(
        Guid serieId,
        Guid seasonId,
        [FromBody] CreateEpisodeDto createEpisodeDto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state for episode creation");
            return BadRequest(ModelState);
        }
        
        logger.LogInformation("Creating episode {EpisodeNumber} for season {SeasonId}", 
            createEpisodeDto.EpisodeNumber, seasonId);
        
        var episode = await episodeService.AddEpisodeToSeasonAsync(
            serieId, seasonId, createEpisodeDto, cancellationToken);
        
        logger.LogInformation("Successfully created episode {EpisodeId}", episode.Id);
        
        return Created($"/api/series/{serieId}/seasons/{seasonId}/episodes/{episode.Id}", episode);
    }

    /// <summary>
    /// Update an episode.
    /// </summary>
    [HttpPut("{serieId}/seasons/{seasonId}/episodes/{episodeId}")]
    [ProducesResponseType(typeof(EpisodeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EpisodeDto>> UpdateEpisodeAsync(
        Guid serieId,
        Guid seasonId,
        Guid episodeId,
        [FromBody] UpdateEpisodeDto updateEpisodeDto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state for episode update: {EpisodeId}", episodeId);
            return BadRequest(ModelState);
        }
        
        logger.LogInformation("Updating episode {EpisodeId}", episodeId);
        
        var episode = await episodeService.UpdateEpisodeAsync(
            serieId, seasonId, episodeId, updateEpisodeDto, cancellationToken);
        
        logger.LogInformation("Successfully updated episode {EpisodeId}", episodeId);
        
        return Ok(episode);
    }

    /// <summary>
    /// Mark an episode as watched.
    /// </summary>
    [HttpPut("{serieId}/seasons/{seasonId}/episodes/{episodeId}/watch")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkEpisodeAsWatchedAsync(
        Guid serieId,
        Guid seasonId,
        Guid episodeId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Marking episode {EpisodeId} as watched", episodeId);
        
        await episodeService.MarkEpisodeAsWatchedAsync(serieId, seasonId, episodeId, cancellationToken);
        
        logger.LogInformation("Successfully marked episode {EpisodeId} as watched", episodeId);
        
        return NoContent();
    }

    /// <summary>
    /// Mark an episode as unwatched.
    /// </summary>
    [HttpPut("{serieId}/seasons/{seasonId}/episodes/{episodeId}/unwatch")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkEpisodeAsUnwatchedAsync(
        Guid serieId,
        Guid seasonId,
        Guid episodeId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Marking episode {EpisodeId} as unwatched", episodeId);
        
        await episodeService.MarkEpisodeAsUnwatchedAsync(serieId, seasonId, episodeId, cancellationToken);
        
        logger.LogInformation("Successfully marked episode {EpisodeId} as unwatched", episodeId);
        
        return NoContent();
    }

    /// <summary>
    /// Rate an episode (0-10).
    /// </summary>
    [HttpPut("{serieId}/seasons/{seasonId}/episodes/{episodeId}/rating")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RateEpisodeAsync(
        Guid serieId,
        Guid seasonId,
        Guid episodeId,
        [FromBody] RateEpisodeDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state for episode rating: {EpisodeId}", episodeId);
            return BadRequest(ModelState);
        }
        
        logger.LogInformation("Rating episode {EpisodeId} with {Rating}", episodeId, dto.Rating);
        
        await episodeService.RateEpisodeAsync(serieId, seasonId, episodeId, dto.Rating, cancellationToken);
        
        logger.LogInformation("Successfully rated episode {EpisodeId}", episodeId);
        
        return NoContent();
    }

    /// <summary>
    /// Delete an episode.
    /// </summary>
    [HttpDelete("{serieId}/seasons/{seasonId}/episodes/{episodeId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEpisodeAsync(
        Guid serieId,
        Guid seasonId,
        Guid episodeId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting episode {EpisodeId}", episodeId);
        
        await episodeService.DeleteEpisodeAsync(serieId, seasonId, episodeId, cancellationToken);
        
        logger.LogInformation("Successfully deleted episode {EpisodeId}", episodeId);
        
        return NoContent();
    }
}