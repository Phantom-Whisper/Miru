using AutoMapper;
using Miru.Application.Exceptions;
using Miru.Contracts.DTOs.Seasons;
using Miru.Contracts.Persistence;
using Miru.Contracts.Services;
using Miru.Domain.Entities;
using Miru.Domain.Exceptions;

namespace Miru.Application.Services;

public class SeasonService: ISeasonService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public SeasonService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<SeasonDto>> GetSeasonsBySerieIdAsync(
        Guid userId,
        Guid serieId,
        CancellationToken cancellationToken = default)
    {
        var serie = await _unitOfWork.Series.GetByIdAsync(serieId, cancellationToken);
        
        if (serie == null)
            throw new NotFoundException("Serie", serieId);
        
        if (serie.UserId != userId)
            throw new ForbiddenException();
        
        var seasons = await _unitOfWork.Seasons.GetBySerieIdAsync(serieId, cancellationToken);
        
        return _mapper.Map<IEnumerable<SeasonDto>>(seasons);
    }
    
    public async Task<SeasonDetailsDto?> GetSeasonByIdAsync(
        Guid userId,
        Guid serieId,
        Guid seasonId,
        CancellationToken cancellationToken = default)
    {
        var serie = await _unitOfWork.Series.GetByIdAsync(serieId, cancellationToken);
        
        if (serie == null)
            throw new NotFoundException("Serie", serieId);
        
        if (serie.UserId != userId)
            throw new ForbiddenException();
        
        var season = await _unitOfWork.Seasons.GetByIdWithEpisodesAsync(seasonId, cancellationToken);
        
        if (season == null || season.SerieId != serieId)
            return null;
        
        return _mapper.Map<SeasonDetailsDto>(season);
    }
    
    public async Task<SeasonDetailsDto> AddSeasonToSerieAsync(
        Guid userId,
        Guid serieId,
        CreateSeasonDto createSeasonDto,
        CancellationToken cancellationToken = default)
    {
        var serie = await _unitOfWork.Series.GetByIdWithSeasonsAsync(serieId, cancellationToken);
        
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
        
        _unitOfWork.Series.Update(serie);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var createdSeason = await _unitOfWork.Seasons.GetByIdWithEpisodesAsync(season.Id, cancellationToken);
        return _mapper.Map<SeasonDetailsDto>(createdSeason);
    }
    
    public async Task<SeasonDetailsDto> UpdateSeasonAsync(
        Guid userId,
        Guid serieId,
        Guid seasonId,
        UpdateSeasonDto updateSeasonDto,
        CancellationToken cancellationToken = default)
    {
        var serie = await _unitOfWork.Series.GetByIdAsync(serieId, cancellationToken);
        
        if (serie == null)
            throw new NotFoundException("Serie", serieId);
        
        if (serie.UserId != userId)
            throw new ForbiddenException();
        
        var season = await _unitOfWork.Seasons.GetByIdAsync(seasonId, cancellationToken);
        
        if (season == null || season.SerieId != serieId)
            throw new NotFoundException("Season", seasonId);
        
        if (updateSeasonDto.SeasonNumber.HasValue)
            season.UpdateSeasonNumber(updateSeasonDto.SeasonNumber.Value);
        
        if (updateSeasonDto.ReleaseDate.HasValue)
            season.UpdateReleaseDate(updateSeasonDto.ReleaseDate.Value);
        
        _unitOfWork.Seasons.Update(season);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var updatedSeason = await _unitOfWork.Seasons.GetByIdWithEpisodesAsync(seasonId, cancellationToken);
        return _mapper.Map<SeasonDetailsDto>(updatedSeason);
    }
    
    public async Task DeleteSeasonAsync(
        Guid userId,
        Guid serieId,
        Guid seasonId,
        CancellationToken cancellationToken = default)
    {
        var serie = await _unitOfWork.Series.GetByIdAsync(serieId, cancellationToken);
        
        if (serie == null)
            throw new NotFoundException("Serie", serieId);
        
        if (serie.UserId != userId)
            throw new ForbiddenException();
        
        var season = await _unitOfWork.Seasons.GetByIdAsync(seasonId, cancellationToken);
        
        if (season == null || season.SerieId != serieId)
            throw new NotFoundException("Season", seasonId);
        
        _unitOfWork.Seasons.Delete(season);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    public async Task MarkSeasonAsWatchedAsync(
        Guid userId,
        Guid serieId,
        Guid seasonId,
        CancellationToken cancellationToken = default)
    {
        var serie = await _unitOfWork.Series.GetByIdAsync(serieId, cancellationToken);
        
        if (serie == null)
            throw new NotFoundException("Serie", serieId);
        
        if (serie.UserId != userId)
            throw new ForbiddenException();
        
        var season = await _unitOfWork.Seasons.GetByIdWithEpisodesAsync(seasonId, cancellationToken);
        
        if (season == null || season.SerieId != serieId)
            throw new NotFoundException("Season", seasonId);
        
        foreach (var episode in season.Episodes.Where(e => !e.Watched))
        {
            episode.MarkAsWatched();
            _unitOfWork.Episodes.Update(episode);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}