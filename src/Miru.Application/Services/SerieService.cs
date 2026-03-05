using AutoMapper;
using Miru.Application.Exceptions;
using Miru.Contracts.Common;
using Miru.Contracts.DTOs.Series;
using Miru.Contracts.Persistence;
using Miru.Contracts.Services;
using Miru.Domain;
using Miru.Domain.Entities;

namespace Miru.Application.Services;

public class SerieService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
    : ISerieService
{
    public async Task<PagingResult<SerieDto>> GetSeriesAsync(
        SerieOrderingCriteria orderingCriteria = SerieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        var result = await unitOfWork.Series.GetSeriesAsync(
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
            Items = mapper.Map<IEnumerable<SerieDto>>(result.Items)
        };
    }

    public async Task<SerieDetailsDto?> GetSerieByIdAsync(
        Guid serieId,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        var serie = await unitOfWork.Series.GetByIdWithSeasonsAndEpisodesAsync(serieId, cancellationToken);
        
        if (serie == null)
            return null;
        
        return serie.UserId != userId ? throw new ForbiddenException() : mapper.Map<SerieDetailsDto>(serie);
    }
    
    public async Task<PagingResult<SerieDto>> GetSeriesByStatusAsync(
        string status,
        SerieOrderingCriteria orderingCriteria = SerieOrderingCriteria.None,
        int pageIndex = 0,
        int countPerPage = 10,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        if (!Enum.TryParse<MediaStatus>(status, out var mediaStatus))
            throw new ValidationException($"Invalid status: {status}. Must be ToWatch, Watching, or Watched.");
        
        var result = await unitOfWork.Series.GetSeriesByStatusAsync(
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
            Items = mapper.Map<IEnumerable<SerieDto>>(result.Items)
        };
    }
    
    public async Task<SerieDetailsDto> CreateSerieAsync(
        CreateSerieDto createSerieDto,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        var serie = Serie.Create(
            userId,
            createSerieDto.Title,
            createSerieDto.ReleaseDate,
            createSerieDto.Description,
            createSerieDto.PosterUrl
        );
        
        await unitOfWork.Series.AddAsync(serie, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return mapper.Map<SerieDetailsDto>(serie);
    }
    
    public async Task<SerieDetailsDto> UpdateSerieAsync(
        Guid serieId,
        UpdateSerieDto updateSerieDto,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        var serie = await unitOfWork.Series.GetByIdAsync(serieId, cancellationToken);
        
        if (serie == null)
            throw new NotFoundException("Serie", serieId);
        
        if (serie.UserId != userId)
            throw new ForbiddenException();
        
        if (!string.IsNullOrWhiteSpace(updateSerieDto.Title))
            serie.UpdateTitle(updateSerieDto.Title);
        
        if (updateSerieDto.ReleaseDate.HasValue)
            serie.UpdateReleaseDate(updateSerieDto.ReleaseDate.Value);
        
        if (updateSerieDto.Description != null)
            serie.UpdateDescription(updateSerieDto.Description);
        
        if (updateSerieDto.PosterUrl != null)
            serie.UpdatePosterUrl(updateSerieDto.PosterUrl);
        
        unitOfWork.Series.Update(serie);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        var updatedSerie = await unitOfWork.Series.GetByIdWithSeasonsAndEpisodesAsync(serieId, cancellationToken);
        return mapper.Map<SerieDetailsDto>(updatedSerie);
    }
    
    public async Task UpdateSerieStatusAsync(
        Guid serieId,
        string status,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        if (!Enum.TryParse<MediaStatus>(status, out var mediaStatus))
            throw new ValidationException($"Invalid status: {status}");
        
        var serie = await unitOfWork.Series.GetByIdAsync(serieId, cancellationToken);
        
        if (serie == null)
            throw new NotFoundException("Serie", serieId);
        
        if (serie.UserId != userId)
            throw new ForbiddenException();
        
        serie.UpdateStatus(mediaStatus);
        
        unitOfWork.Series.Update(serie);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    public async Task RateSerieAsync(
        Guid serieId,
        double rating,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        var serie = await unitOfWork.Series.GetByIdAsync(serieId, cancellationToken);
        
        if (serie == null)
            throw new NotFoundException("Serie", serieId);
        
        if (serie.UserId != userId)
            throw new ForbiddenException();
        
        serie.SetRating(rating);
        
        unitOfWork.Series.Update(serie);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    public async Task DeleteSerieAsync(
        Guid serieId,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        var serie = await unitOfWork.Series.GetByIdAsync(serieId, cancellationToken);
        
        if (serie == null)
            throw new NotFoundException("Serie", serieId);
        
        if (serie.UserId != userId)
            throw new ForbiddenException();
        
        unitOfWork.Series.Delete(serie);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}