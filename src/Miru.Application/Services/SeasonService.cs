using AutoMapper;
using Miru.Application.Exceptions;
using Miru.Shared.DTOs.Seasons;
using Miru.Shared.Services;
using Miru.Domain.Entities;
using Miru.Domain.Exceptions;
using Miru.Infrastructure.Persistence.UnitOfWork;

namespace Miru.Application.Services;

public class SeasonService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
    : ISeasonService
{
    public async Task<IEnumerable<SeasonDto>> GetSeasonsBySerieIdAsync(
        Guid serieId,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        var serie = await unitOfWork.Series.GetByIdAsync(serieId, cancellationToken);
        
        if (serie == null)
            throw new NotFoundException("Serie", serieId);
        
        if (serie.UserId != userId)
            throw new ForbiddenException();
        
        var seasons = await unitOfWork.Seasons.GetBySerieIdAsync(serieId, cancellationToken);
        
        return mapper.Map<IEnumerable<SeasonDto>>(seasons);
    }
    
    public async Task<SeasonDetailsDto?> GetSeasonByIdAsync(
        Guid serieId,
        Guid seasonId,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        var serie = await unitOfWork.Series.GetByIdAsync(serieId, cancellationToken);
        
        if (serie == null)
            throw new NotFoundException("Serie", serieId);
        
        if (serie.UserId != userId)
            throw new ForbiddenException();
        
        var season = await unitOfWork.Seasons.GetByIdWithEpisodesAsync(seasonId, cancellationToken);
        
        if (season == null || season.SerieId != serieId)
            return null;
        
        return mapper.Map<SeasonDetailsDto>(season);
    }
    
    public async Task<SeasonDetailsDto> AddSeasonToSerieAsync(
        Guid serieId,
        CreateSeasonDto createSeasonDto,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        var serie = await unitOfWork.Series.GetByIdWithSeasonsAsync(serieId, cancellationToken);
        
        if (serie == null)
            throw new NotFoundException("Serie", serieId);
        
        if (serie.UserId != userId)
            throw new ForbiddenException();
        
        var season = Season.Create(
            createSeasonDto.SeasonNumber,
            createSeasonDto.ReleaseDate,
            serieId
        );
        
        try
        {
            serie.AddSeason(season);
        }
        catch (DomainException ex)
        {
            throw new ValidationException(ex.Message);
        }
        
        unitOfWork.Series.Update(serie);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        var createdSeason = await unitOfWork.Seasons.GetByIdWithEpisodesAsync(season.Id, cancellationToken);
        return mapper.Map<SeasonDetailsDto>(createdSeason);
    }
    
    public async Task<SeasonDetailsDto> UpdateSeasonAsync(
        Guid serieId,
        Guid seasonId,
        UpdateSeasonDto updateSeasonDto,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        var serie = await unitOfWork.Series.GetByIdAsync(serieId, cancellationToken);
        
        if (serie == null)
            throw new NotFoundException("Serie", serieId);
        
        if (serie.UserId != userId)
            throw new ForbiddenException();
        
        var season = await unitOfWork.Seasons.GetByIdAsync(seasonId, cancellationToken);
        
        if (season == null || season.SerieId != serieId)
            throw new NotFoundException("Season", seasonId);
        
        if (updateSeasonDto.SeasonNumber.HasValue)
            season.UpdateSeasonNumber(updateSeasonDto.SeasonNumber.Value);
        
        if (updateSeasonDto.ReleaseDate.HasValue)
            season.UpdateReleaseDate(updateSeasonDto.ReleaseDate.Value);
        
        unitOfWork.Seasons.Update(season);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        var updatedSeason = await unitOfWork.Seasons.GetByIdWithEpisodesAsync(seasonId, cancellationToken);
        return mapper.Map<SeasonDetailsDto>(updatedSeason);
    }
    
    public async Task DeleteSeasonAsync(
        Guid serieId,
        Guid seasonId,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        var serie = await unitOfWork.Series.GetByIdAsync(serieId, cancellationToken);
        
        if (serie == null)
            throw new NotFoundException("Serie", serieId);
        
        if (serie.UserId != userId)
            throw new ForbiddenException();
        
        var season = await unitOfWork.Seasons.GetByIdAsync(seasonId, cancellationToken);
        
        if (season == null || season.SerieId != serieId)
            throw new NotFoundException("Season", seasonId);
        
        unitOfWork.Seasons.Delete(season);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    public async Task MarkSeasonAsWatchedAsync(
        Guid serieId,
        Guid seasonId,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        var serie = await unitOfWork.Series.GetByIdAsync(serieId, cancellationToken);
        
        if (serie == null)
            throw new NotFoundException("Serie", serieId);
        
        if (serie.UserId != userId)
            throw new ForbiddenException();
        
        var season = await unitOfWork.Seasons.GetByIdWithEpisodesAsync(seasonId, cancellationToken);
        
        if (season == null || season.SerieId != serieId)
            throw new NotFoundException("Season", seasonId);
        
        foreach (var episode in season.Episodes.Where(e => !e.Watched))
        {
            episode.MarkAsWatched();
            unitOfWork.Episodes.Update(episode);
        }
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}