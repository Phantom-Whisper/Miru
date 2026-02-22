using AutoMapper;
using Miru.Application.Exceptions;
using Miru.Contracts.Common;
using Miru.Contracts.DTOs.Episodes;
using Miru.Contracts.Persistence;
using Miru.Contracts.Services;
using Miru.Domain.Entities;
using Miru.Domain.Exceptions;

namespace Miru.Application.Services;

public class EpisodeService: IEpisodeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public EpisodeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<PagingResult<EpisodeDto>> GetEpisodesBySeasonIdAsync(
        Guid userId,
        Guid serieId,
        Guid seasonId,
        EpisodeOrderingCriteria orderingCriteria = EpisodeOrderingCriteria.ByEpisodeNumber,
        int pageIndex = 0,
        int countPerPage = 50,
        CancellationToken cancellationToken = default)
    {
        await ValidateUserOwnsSerieAsync(userId, serieId, cancellationToken);
        await ValidateSeasonBelongsToSerieAsync(serieId, seasonId, cancellationToken);
        
        var result = await _unitOfWork.Episodes.GetBySeasonIdAsync(
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
            Items = _mapper.Map<IEnumerable<EpisodeDto>>(result.Items)
        };
    }
    
    public async Task<EpisodeDto?> GetEpisodeByIdAsync(
        Guid userId,
        Guid serieId,
        Guid seasonId,
        Guid episodeId,
        CancellationToken cancellationToken = default)
    {
        await ValidateUserOwnsSerieAsync(userId, serieId, cancellationToken);
        await ValidateSeasonBelongsToSerieAsync(serieId, seasonId, cancellationToken);
        
        var episode = await _unitOfWork.Episodes.GetByIdAsync(episodeId, cancellationToken);
        
        if (episode == null || episode.SeasonId != seasonId)
            return null;
        
        return _mapper.Map<EpisodeDto>(episode);
    }
    
    public async Task<EpisodeDto?> GetNextUnwatchedEpisodeAsync(
        Guid userId,
        Guid serieId,
        CancellationToken cancellationToken = default)
    {
        await ValidateUserOwnsSerieAsync(userId, serieId, cancellationToken);
        
        var episode = await _unitOfWork.Episodes.GetNextUnwatchedEpisodeAsync(userId, serieId, cancellationToken);
        
        if (episode == null)
            return null;
        
        return _mapper.Map<EpisodeDto>(episode);
    }
    
    public async Task<PagingResult<EpisodeDto>> GetRecentlyWatchedEpisodesAsync(
        Guid userId,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.Episodes.GetRecentlyWatchedAsync(
            userId,
            pageIndex,
            countPerPage,
            cancellationToken);
        
        return new PagingResult<EpisodeDto>
        {
            TotalCount = result.TotalCount,
            PageIndex = result.PageIndex,
            CountPerPage = result.CountPerPage,
            Items = _mapper.Map<IEnumerable<EpisodeDto>>(result.Items)
        };
    }
    
    public async Task<EpisodeDto> AddEpisodeToSeasonAsync(
        Guid userId,
        Guid serieId,
        Guid seasonId,
        CreateEpisodeDto createEpisodeDto,
        CancellationToken cancellationToken = default)
    {
        await ValidateUserOwnsSerieAsync(userId, serieId, cancellationToken);
        
        var season = await _unitOfWork.Seasons.GetByIdWithEpisodesAsync(seasonId, cancellationToken);
        
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
        
        _unitOfWork.Seasons.Update(season);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return _mapper.Map<EpisodeDto>(episode);
    }

    public async Task<EpisodeDto> UpdateEpisodeAsync(
        Guid userId,
        Guid serieId,
        Guid seasonId,
        Guid episodeId,
        UpdateEpisodeDto updateEpisodeDto,
        CancellationToken cancellationToken = default)
    {
        await ValidateUserOwnsSerieAsync(userId, serieId, cancellationToken);
        await ValidateSeasonBelongsToSerieAsync(serieId, seasonId, cancellationToken);
        
        var episode = await _unitOfWork.Episodes.GetByIdAsync(episodeId, cancellationToken);
        
        if (episode == null || episode.SeasonId != seasonId)
            throw new NotFoundException("Episode", episodeId);
        
        if (updateEpisodeDto.EpisodeNumber.HasValue)
            episode.UpdateEpisodeNumber(updateEpisodeDto.EpisodeNumber.Value);
        
        if (!string.IsNullOrWhiteSpace(updateEpisodeDto.Title))
            episode.UpdateTitle(updateEpisodeDto.Title);
        
        if (updateEpisodeDto.DurationMinutes.HasValue)
            episode.UpdateDuration(TimeSpan.FromMinutes(updateEpisodeDto.DurationMinutes.Value));
        
        _unitOfWork.Episodes.Update(episode);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return _mapper.Map<EpisodeDto>(episode);
    }
    
    public async Task MarkEpisodeAsWatchedAsync(
        Guid userId,
        Guid serieId,
        Guid seasonId,
        Guid episodeId,
        CancellationToken cancellationToken = default)
    {
        await ValidateUserOwnsSerieAsync(userId, serieId, cancellationToken);
        await ValidateSeasonBelongsToSerieAsync(serieId, seasonId, cancellationToken);
        
        var episode = await _unitOfWork.Episodes.GetByIdAsync(episodeId, cancellationToken);
        
        if (episode == null || episode.SeasonId != seasonId)
            throw new NotFoundException("Episode", episodeId);
        
        episode.MarkAsWatched();
        
        _unitOfWork.Episodes.Update(episode);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    public async Task MarkEpisodeAsUnwatchedAsync(
        Guid userId,
        Guid serieId,
        Guid seasonId,
        Guid episodeId,
        CancellationToken cancellationToken = default)
    {
        await ValidateUserOwnsSerieAsync(userId, serieId, cancellationToken);
        await ValidateSeasonBelongsToSerieAsync(serieId, seasonId, cancellationToken);
        
        var episode = await _unitOfWork.Episodes.GetByIdAsync(episodeId, cancellationToken);
        
        if (episode == null || episode.SeasonId != seasonId)
            throw new NotFoundException("Episode", episodeId);
        
        episode.MarkAsUnwatched();
        
        _unitOfWork.Episodes.Update(episode);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    public async Task RateEpisodeAsync(
        Guid userId,
        Guid serieId,
        Guid seasonId,
        Guid episodeId,
        double rating,
        CancellationToken cancellationToken = default)
    {
        await ValidateUserOwnsSerieAsync(userId, serieId, cancellationToken);
        await ValidateSeasonBelongsToSerieAsync(serieId, seasonId, cancellationToken);
        
        var episode = await _unitOfWork.Episodes.GetByIdAsync(episodeId, cancellationToken);
        
        if (episode == null || episode.SeasonId != seasonId)
            throw new NotFoundException("Episode", episodeId);
        
        episode.SetRating(rating);
        
        _unitOfWork.Episodes.Update(episode);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    public async Task DeleteEpisodeAsync(
        Guid userId,
        Guid serieId,
        Guid seasonId,
        Guid episodeId,
        CancellationToken cancellationToken = default)
    {
        await ValidateUserOwnsSerieAsync(userId, serieId, cancellationToken);
        await ValidateSeasonBelongsToSerieAsync(serieId, seasonId, cancellationToken);
        
        var episode = await _unitOfWork.Episodes.GetByIdAsync(episodeId, cancellationToken);
        
        if (episode == null || episode.SeasonId != seasonId)
            throw new NotFoundException("Episode", episodeId);
        
        _unitOfWork.Episodes.Delete(episode);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    private async Task ValidateUserOwnsSerieAsync(Guid userId, Guid serieId, CancellationToken cancellationToken)
    {
        var serie = await _unitOfWork.Series.GetByIdAsync(serieId, cancellationToken);
        
        if (serie == null)
            throw new NotFoundException("Serie", serieId);
        
        if (serie.UserId != userId)
            throw new ForbiddenException();
    }
    
    private async Task ValidateSeasonBelongsToSerieAsync(Guid serieId, Guid seasonId, CancellationToken cancellationToken)
    {
        var season = await _unitOfWork.Seasons.GetByIdAsync(seasonId, cancellationToken);
        
        if (season == null)
            throw new NotFoundException("Season", seasonId);
        
        if (season.SerieId != serieId)
            throw new ValidationException($"Season {seasonId} does not belong to Serie {serieId}");
    }
}