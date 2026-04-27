using Miru.Shared.DTOs.Movies;
using Miru.Domain.Entities;


namespace Miru.Application.Mappings;

public static class MovieMappings
{
    public static MovieDto ToDto(this Movie src) => new()
    {
        Id        = src.Id,
        Title     = src.Title,
        PosterUrl = src.PosterUrl,
        Status    = src.Status,
        Rating    = src.Rating,
        Duration  = (int)src.Duration.TotalMinutes,
    };

    public static MovieDetailsDto ToDetailsDto(this Movie src) => new()
    {
        Id          = src.Id,
        Title       = src.Title,
        PosterUrl   = src.PosterUrl,
        Status      = src.Status,
        Rating      = src.Rating,
        Duration    = (int)src.Duration.TotalMinutes,
        Description = src.Description,
        ReleaseDate = src.ReleaseDate,
        AddedAt     = src.AddedAt,
        WatchedAt   = src.WatchedAt,
    };
    
    public static Movie ToEntity(this CreateMovieDto src, Guid userId) =>
        Movie.Create(
            userId:      userId,
            title:       src.Title,
            duration:    TimeSpan.FromMinutes(src.Duration),
            releaseDate: src.ReleaseDate,
            description: src.Description,
            posterUrl:   src.PosterUrl
        );

    public static void ApplyUpdate(this UpdateMovieDto src, Movie movie)
    {
        if (src.Title is not null)       movie.UpdateTitle(src.Title);
        if (src.Duration is not null)    movie.UpdateDuration(TimeSpan.FromMinutes(src.Duration.Value));
        if (src.ReleaseDate is not null) movie.UpdateReleaseDate(src.ReleaseDate.Value);
        if (src.Description is not null) movie.UpdateDescription(src.Description);
        if (src.PosterUrl is not null)   movie.UpdatePosterUrl(src.PosterUrl);
    }
}