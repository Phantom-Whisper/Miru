using AutoMapper;
using Miru.Contracts.DTOs.Episodes;
using Miru.Contracts.DTOs.Movies;
using Miru.Contracts.DTOs.Seasons;
using Miru.Contracts.DTOs.Series;
using Miru.Domain.Entities;

namespace Miru.Application.Mappings;

public class MappingProfile : Profile
{
    /// <summary>
    /// Initializes all mapping configurations.
    /// <remarks>
    /// Each aggregate has its own configuration method for better readability and scalability.
    /// </remarks>
    /// </summary>
    public MappingProfile()
    {
        ConfigureMovieMappings();
        ConfigureSerieMappings();
        ConfigureSeasonMappings();
        ConfigureEpisodeMappings();
    }

    #region Movie Mappings

    /// <summary>
    /// Configures mappings related to the Movie aggregate.
    /// <remarks>
    /// Duration (TimeSpan) is converted to total minutes (int).
    /// Status enum is converted to string representation.
    /// </remarks>
    /// </summary>
    private void ConfigureMovieMappings()
    {
        CreateMap<Movie, MovieDto>()
            .ForMember(dest => dest.Duration,
                opt => opt.MapFrom(src => (int)src.Duration.TotalMinutes));

        CreateMap<Movie, MovieDetailsDto>()
            .IncludeBase<Movie, MovieDto>();
    }
    #endregion
    
    #region Serie Mappings

    /// <summary>
    /// Configures mappings related to the Serie aggregate.
    /// </summary>
    private void ConfigureSerieMappings()
    {
        CreateMap<Serie, SerieDto>()
            .ForMember(dest => dest.SeasonsCount,
                opt => opt.MapFrom(src => src.Seasons.Count))
            .ForMember(dest => dest.TotalEpisodesCount,
                opt => opt.MapFrom(src => src.Seasons.Sum(season => season.Episodes.Count)))
            .ForMember(dest => dest.WatchedEpisodesCount,
                opt => opt.MapFrom(src => src.Seasons.Sum(season => season.Episodes.Count(e => e.Watched))))
            .ForMember(dest => dest.ProgressPercentage,
                opt => opt.MapFrom(src => CalculateSeasonProgress(src)));
        
        CreateMap<Serie, SerieDetailsDto>()
            .IncludeBase<Serie, SerieDto>()
            .ForMember(dest => dest.Seasons,
                opt => opt.MapFrom(src => src.Seasons));
    }

    /// <summary>
    /// Calculates the percentage of episodes watched in a serie.
    /// </summary>
    /// <param name="serie">The serie entity.</param>
    /// <returns>A percentage representing watched progression.</returns>
    private double CalculateSeasonProgress(Serie serie)
    {
        var totalEpisode =  serie.Seasons.Sum(s => s.Episodes.Count);
        if (totalEpisode == 0)
            return 0;
        
        var watchedEpisode = serie.Seasons.Sum(s => s.Episodes.Count(e => e.Watched));
        
        return Math.Round((double)watchedEpisode / totalEpisode * 100, 2);
    }
    #endregion
    
    #region Season Mappings
    
    /// <summary>
    /// Configures mappings related to the Season entity.
    /// </summary>
    private void ConfigureSeasonMappings()
    {
        CreateMap<Season, SeasonDto>()
            .ForMember(dest => dest.EpisodesCount, 
                opt => opt.MapFrom(src => src.Episodes.Count))
            .ForMember(dest => dest.WatchedEpisodesCount, 
                opt => opt.MapFrom(src => src.Episodes.Count(e => e.Watched)))
            .ForMember(dest => dest.ProgressPercentage,
                opt => opt.MapFrom(src => CalculateSeasonProgress(src)));
        
        CreateMap<Season, SeasonDetailsDto>()
            .IncludeBase<Season, SeasonDto>()
            .ForMember(dest => dest.Episodes,
                opt => opt.MapFrom(src => src.Episodes));
    }
    
    /// <summary>
    /// Calculates the percentage of watched episodes within a season.
    /// </summary>
    /// <param name="season">The season entity.</param>
    /// <returns>A percentage representing watched progression.</returns>
    private double CalculateSeasonProgress(Season season)
    {
        if (season.Episodes.Count == 0) return 0;
        
        var watchedCount = season.Episodes.Count(e => e.Watched);
        return Math.Round((double)watchedCount / season.Episodes.Count * 100, 2);
    }
    #endregion
    
    #region Episode Mappings

    /// <summary>
    /// Configures mappings related to the Episode entity.
    /// </summary>
    private void ConfigureEpisodeMappings()
    {
        CreateMap<Episode, EpisodeDto>()
            .ForMember(dest => dest.Duration, 
                opt => opt.MapFrom(src => (int)src.Duration.TotalMinutes));
    }
    #endregion
}