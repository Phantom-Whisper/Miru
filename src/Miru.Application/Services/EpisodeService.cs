using Miru.Application.Exceptions;
using Miru.Application.Interfaces;
using Miru.Application.Mappings;
using Miru.Shared.Common;
using Miru.Shared.DTOs.Episodes;
using Miru.Domain.Entities;
using Miru.Domain.Exceptions;
using Miru.Infrastructure.Persistence.UnitOfWork;

namespace Miru.Application.Services;

public class EpisodeService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    : IEpisodeService
{
    public async Task<PagingResult<EpisodeDto>> GetEpisodesBySeasonIdAsync(
        Guid serieId,
        Guid seasonId,
        EpisodeOrderingCriteria orderingCriteria = EpisodeOrderingCriteria.ByEpisodeNumber,
        int pageIndex = 0,
        int countPerPage = 50,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        await ValidateUserOwnsSerieAsync(userId, serieId, cancellationToken);
        await ValidateSeasonBelongsToSerieAsync(serieId, seasonId, cancellationToken);
        
        var result = await unitOfWork.Episodes.GetBySeasonIdAsync(
            seasonId,
            orderingCriteria,
            pageIndex,
            countPerPage,
            cancellationToken);
        
        return new PagingResult<EpisodeDto>
        {
            TotalCount = result.TotalCount,
            PageIndex = result.PageIndex,
            CountPerPage = result.CountPerPage,
            Items = result.Items.Select(e => e.ToDto())
        };
    }
    
    public async Task<EpisodeDto?> GetEpisodeByIdAsync(
        Guid serieId,
        Guid seasonId,
        Guid episodeId,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        await ValidateUserOwnsSerieAsync(userId, serieId, cancellationToken);
        await ValidateSeasonBelongsToSerieAsync(serieId, seasonId, cancellationToken);
        
        var episode = await unitOfWork.Episodes.GetByIdAsync(episodeId, cancellationToken);
        
        if (episode == null || episode.SeasonId != seasonId)
            return null;
        
        return episode.ToDto();
    }
    
    public async Task<EpisodeDto?> GetNextUnwatchedEpisodeAsync(
        Guid serieId,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        await ValidateUserOwnsSerieAsync(userId, serieId, cancellationToken);
        
        var episode = await unitOfWork.Episodes.GetNextUnwatchedEpisodeAsync(userId, serieId, cancellationToken);

        return episode?.ToDto();
    }
    
    public async Task<PagingResult<EpisodeDto>> GetRecentlyWatchedEpisodesAsync(
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        var result = await unitOfWork.Episodes.GetRecentlyWatchedAsync(
            userId,
            pageIndex,
            countPerPage,
            cancellationToken);
        
        return new PagingResult<EpisodeDto>
        {
            TotalCount = result.TotalCount,
            PageIndex = result.PageIndex,
            CountPerPage = result.CountPerPage,
            Items = result.Items.Select(e => e.ToDto())
        };
    }
    
    public async Task<EpisodeDto> AddEpisodeToSeasonAsync(
        Guid serieId,
        Guid seasonId,
        CreateEpisodeDto createEpisodeDto,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        await ValidateUserOwnsSerieAsync(userId, serieId, cancellationToken);
        
        var season = await unitOfWork.Seasons.GetByIdWithEpisodesAsync(seasonId, cancellationToken);
        
        if (season == null || season.SerieId != serieId)
            throw new NotFoundException("Season", seasonId);
        
        var episode = Episode.Create(
            createEpisodeDto.EpisodeNumber,
            createEpisodeDto.Title,
            TimeSpan.FromMinutes(createEpisodeDto.DurationMinutes),
            seasonId
        );
        
        try
        {
            season.AddEpisode(episode);
        }
        catch (DomainException ex)
        {
            throw new ValidationException(ex.Message);
        }
        
        unitOfWork.Seasons.Update(season);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return episode.ToDto();
    }

    public async Task<EpisodeDto> UpdateEpisodeAsync(
        Guid serieId,
        Guid seasonId,
        Guid episodeId,
        UpdateEpisodeDto updateEpisodeDto,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        await ValidateUserOwnsSerieAsync(userId, serieId, cancellationToken);
        await ValidateSeasonBelongsToSerieAsync(serieId, seasonId, cancellationToken);
        
        var episode = await unitOfWork.Episodes.GetByIdAsync(episodeId, cancellationToken);
        
        if (episode == null || episode.SeasonId != seasonId)
            throw new NotFoundException("Episode", episodeId);
        
        if (updateEpisodeDto.EpisodeNumber.HasValue)
            episode.UpdateEpisodeNumber(updateEpisodeDto.EpisodeNumber.Value);
        
        if (!string.IsNullOrWhiteSpace(updateEpisodeDto.Title))
            episode.UpdateTitle(updateEpisodeDto.Title);
        
        if (updateEpisodeDto.DurationMinutes.HasValue)
            episode.UpdateDuration(TimeSpan.FromMinutes(updateEpisodeDto.DurationMinutes.Value));
        
        unitOfWork.Episodes.Update(episode);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return episode.ToDto();
    }
    
    public async Task MarkEpisodeAsWatchedAsync(
        Guid serieId,
        Guid seasonId,
        Guid episodeId,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        await ValidateUserOwnsSerieAsync(userId, serieId, cancellationToken);
        await ValidateSeasonBelongsToSerieAsync(serieId, seasonId, cancellationToken);
        
        var episode = await unitOfWork.Episodes.GetByIdAsync(episodeId, cancellationToken);
        
        if (episode == null || episode.SeasonId != seasonId)
            throw new NotFoundException("Episode", episodeId);
        
        episode.MarkAsWatched();
        
        unitOfWork.Episodes.Update(episode);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    public async Task MarkEpisodeAsUnwatchedAsync(
        Guid serieId,
        Guid seasonId,
        Guid episodeId,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        await ValidateUserOwnsSerieAsync(userId, serieId, cancellationToken);
        await ValidateSeasonBelongsToSerieAsync(serieId, seasonId, cancellationToken);
        
        var episode = await unitOfWork.Episodes.GetByIdAsync(episodeId, cancellationToken);
        
        if (episode == null || episode.SeasonId != seasonId)
            throw new NotFoundException("Episode", episodeId);
        
        episode.MarkAsUnwatched();
        
        unitOfWork.Episodes.Update(episode);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    public async Task RateEpisodeAsync(
        Guid serieId,
        Guid seasonId,
        Guid episodeId,
        double rating,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        await ValidateUserOwnsSerieAsync(userId, serieId, cancellationToken);
        await ValidateSeasonBelongsToSerieAsync(serieId, seasonId, cancellationToken);
        
        var episode = await unitOfWork.Episodes.GetByIdAsync(episodeId, cancellationToken);
        
        if (episode == null || episode.SeasonId != seasonId)
            throw new NotFoundException("Episode", episodeId);
        
        episode.SetRating(rating);
        
        unitOfWork.Episodes.Update(episode);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    public async Task DeleteEpisodeAsync(
        Guid serieId,
        Guid seasonId,
        Guid episodeId,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        await ValidateUserOwnsSerieAsync(userId, serieId, cancellationToken);
        await ValidateSeasonBelongsToSerieAsync(serieId, seasonId, cancellationToken);
        
        var episode = await unitOfWork.Episodes.GetByIdAsync(episodeId, cancellationToken);
        
        if (episode == null || episode.SeasonId != seasonId)
            throw new NotFoundException("Episode", episodeId);
        
        unitOfWork.Episodes.Delete(episode);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    private async Task ValidateUserOwnsSerieAsync(
        Guid userId,
        Guid serieId,
        CancellationToken cancellationToken)
    {
        var serie = await unitOfWork.Series.GetByIdAsync(serieId, cancellationToken);
        
        if (serie == null)
            throw new NotFoundException("Serie", serieId);
        
        if (serie.UserId != userId)
            throw new ForbiddenException();
    }
    
    private async Task ValidateSeasonBelongsToSerieAsync(
        Guid serieId, 
        Guid seasonId, 
        CancellationToken cancellationToken)
    {
        var season = await unitOfWork.Seasons.GetByIdAsync(seasonId, cancellationToken);
        
        if (season == null)
            throw new NotFoundException("Season", seasonId);
        
        if (season.SerieId != serieId)
            throw new ValidationException($"Season {seasonId} does not belong to Serie {serieId}");
    }
}