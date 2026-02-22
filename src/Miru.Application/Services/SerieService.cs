using AutoMapper;
using Miru.Application.Exceptions;
using Miru.Contracts.Common;
using Miru.Contracts.DTOs.Series;
using Miru.Contracts.Persistence;
using Miru.Contracts.Services;
using Miru.Domain;
using Miru.Domain.Entities;

namespace Miru.Application.Services;

public class SerieService : ISerieService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public SerieService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<PagingResult<SerieDto>> GetSeriesAsync(
        Guid userId,
        SerieOrderingCriteria orderingCriteria = SerieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.Series.GetSeriesAsync(
            userId,
            orderingCriteria,
            pageIndex,
            countPerPage,
            cancellationToken);
        
        return new PagingResult<SerieDto>
        {
            TotalCount = result.TotalCount,
            PageIndex = result.PageIndex,
            CountPerPage = result.CountPerPage,
            Items = _mapper.Map<IEnumerable<SerieDto>>(result.Items)
        };
    }

    public async Task<SerieDetailsDto?> GetSerieByIdAsync(
        Guid userId,
        Guid serieId,
        CancellationToken cancellationToken = default)
    {
        var serie = await _unitOfWork.Series.GetByIdWithSeasonsAndEpisodesAsync(serieId, cancellationToken);
        
        if (serie == null)
            return null;
        
        return serie.UserId != userId ? throw new ForbiddenException() : _mapper.Map<SerieDetailsDto>(serie);
    }
    
    public async Task<PagingResult<SerieDto>> GetSeriesByStatusAsync(
        Guid userId,
        string status,
        SerieOrderingCriteria orderingCriteria = SerieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<MediaStatus>(status, out var mediaStatus))
            throw new ValidationException($"Invalid status: {status}. Must be ToWatch, Watching, or Watched.");
        
        var result = await _unitOfWork.Series.GetSeriesByStatusAsync(
            userId,
            mediaStatus,
            orderingCriteria,
            pageIndex,
            countPerPage,
            cancellationToken);
        
        return new PagingResult<SerieDto>
        {
            TotalCount = result.TotalCount,
            PageIndex = result.PageIndex,
            CountPerPage = result.CountPerPage,
            Items = _mapper.Map<IEnumerable<SerieDto>>(result.Items)
        };
    }
    
    public async Task<SerieDetailsDto> CreateSerieAsync(
        Guid userId,
        CreateSerieDto createSerieDto,
        CancellationToken cancellationToken = default)
    {
        var serie = Serie.Create(
            userId,
            createSerieDto.Title,
            createSerieDto.ReleaseDate,
            createSerieDto.Description,
            createSerieDto.PosterUrl
        );
        
        await _unitOfWork.Series.AddAsync(serie, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return _mapper.Map<SerieDetailsDto>(serie);
    }
    
    public async Task<SerieDetailsDto> UpdateSerieAsync(
        Guid userId,
        Guid serieId,
        UpdateSerieDto updateSerieDto,
        CancellationToken cancellationToken = default)
    {
        var serie = await _unitOfWork.Series.GetByIdAsync(serieId, cancellationToken);
        
        if (serie == null)
            throw new NotFoundException("Serie", serieId);
        
        if (serie.UserId != userId)
            throw new ForbiddenException();
        
        // Mettre à jour les propriétés
        if (!string.IsNullOrWhiteSpace(updateSerieDto.Title))
            serie.UpdateTitle(updateSerieDto.Title);
        
        if (updateSerieDto.ReleaseDate.HasValue)
            serie.UpdateReleaseDate(updateSerieDto.ReleaseDate.Value);
        
        if (updateSerieDto.Description != null)
            serie.UpdateDescription(updateSerieDto.Description);
        
        if (updateSerieDto.PosterUrl != null)
            serie.UpdatePosterUrl(updateSerieDto.PosterUrl);
        
        _unitOfWork.Series.Update(serie);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        // Recharger avec les saisons/épisodes pour le DTO
        var updatedSerie = await _unitOfWork.Series.GetByIdWithSeasonsAndEpisodesAsync(serieId, cancellationToken);
        return _mapper.Map<SerieDetailsDto>(updatedSerie);
    }
    
    public async Task UpdateSerieStatusAsync(
        Guid userId,
        Guid serieId,
        string status,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<MediaStatus>(status, out var mediaStatus))
            throw new ValidationException($"Invalid status: {status}");
        
        var serie = await _unitOfWork.Series.GetByIdAsync(serieId, cancellationToken);
        
        if (serie == null)
            throw new NotFoundException("Serie", serieId);
        
        if (serie.UserId != userId)
            throw new ForbiddenException();
        
        serie.UpdateStatus(mediaStatus);
        
        _unitOfWork.Series.Update(serie);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    public async Task RateSerieAsync(
        Guid userId,
        Guid serieId,
        double rating,
        CancellationToken cancellationToken = default)
    {
        var serie = await _unitOfWork.Series.GetByIdAsync(serieId, cancellationToken);
        
        if (serie == null)
            throw new NotFoundException("Serie", serieId);
        
        if (serie.UserId != userId)
            throw new ForbiddenException();
        
        serie.SetRating(rating);
        
        _unitOfWork.Series.Update(serie);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    public async Task DeleteSerieAsync(
        Guid userId,
        Guid serieId,
        CancellationToken cancellationToken = default)
    {
        var serie = await _unitOfWork.Series.GetByIdAsync(serieId, cancellationToken);
        
        if (serie == null)
            throw new NotFoundException("Serie", serieId);
        
        if (serie.UserId != userId)
            throw new ForbiddenException();
        
        // EF Core cascade delete s'occupe des saisons et épisodes
        _unitOfWork.Series.Delete(serie);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}