using Miru.Application.Mappings;
using Miru.Domain.Entities;
using Miru.Shared.Common.Enums;

namespace Miru.Test.Application;

public class MappingTests
{
    [Fact]
    public void Movie_To_MovieDto_Should_Map_Correctly()
    {
        var movie = Movie.Create(
            Guid.NewGuid(),
            "Inception",
            TimeSpan.FromMinutes(148),
            new DateOnly(2010, 7, 16),
            "Dream infiltration",
            "https://poster.jpg"
        );
        movie.UpdateStatus(MediaStatus.Watched);
        movie.SetRating(9.0);

        var dto = movie.ToDto();

        Assert.Equal(movie.Id, dto.Id);
        Assert.Equal("Inception", dto.Title);
        Assert.Equal(148, dto.Duration);
        Assert.Equal(MediaStatus.Watched, dto.Status);
        Assert.Equal(9.0, dto.Rating);
    }

    [Fact]
    public void Movie_To_MovieDetailsDto_Should_Map_Correctly()
    {
        var movie = Movie.Create(
            Guid.NewGuid(),
            "Interstellar",
            TimeSpan.FromMinutes(169),
            new DateOnly(2014, 11, 7),
            "Space exploration"
        );

        var dto = movie.ToDetailsDto();

        Assert.Equal(movie.Id, dto.Id);
        Assert.Equal(169, dto.Duration);
        Assert.Equal(movie.ReleaseDate, dto.ReleaseDate);
        Assert.Equal(movie.Description, dto.Description);
    }

    [Fact]
    public void Episode_To_EpisodeDto_Should_Map_Duration()
    {
        var episode = Episode.Create(1, "Pilot", TimeSpan.FromMinutes(58), Guid.NewGuid());

        var dto = episode.ToDto();

        Assert.Equal(1, dto.EpisodeNumber);
        Assert.Equal("Pilot", dto.Title);
        Assert.Equal(58, dto.Duration);
    }

    [Fact]
    public void Season_To_SeasonDto_Should_Calculate_Progress()
    {
        var season = Season.Create(1, new DateOnly(2024, 1, 1), Guid.NewGuid());
        var ep1 = Episode.Create(1, "Ep1", TimeSpan.FromMinutes(45), season.Id);
        ep1.MarkAsWatched();
        var ep2 = Episode.Create(2, "Ep2", TimeSpan.FromMinutes(45), season.Id);
        season.AddEpisode(ep1);
        season.AddEpisode(ep2);

        var dto = season.ToDto();

        Assert.Equal(2, dto.EpisodesCount);
        Assert.Equal(1, dto.WatchedEpisodesCount);
        Assert.Equal(50.00, dto.ProgressPercentage);
    }

    [Fact]
    public void Season_To_SeasonDto_With_No_Episodes_Should_Return_Zero_Progress()
    {
        var season = Season.Create(1, new DateOnly(2024, 1, 1), Guid.NewGuid());

        var dto = season.ToDto();

        Assert.Equal(0, dto.EpisodesCount);
        Assert.Equal(0, dto.WatchedEpisodesCount);
        Assert.Equal(0, dto.ProgressPercentage);
    }

    [Fact]
    public void Season_To_SeasonDetailsDto_Should_Map_Episodes()
    {
        var season = Season.Create(1, new DateOnly(2024, 1, 1), Guid.NewGuid());
        season.AddEpisode(Episode.Create(1, "Ep1", TimeSpan.FromMinutes(45), season.Id));

        var dto = season.ToDetailsDto();

        Assert.Single(dto.Episodes);
    }

    [Fact]
    public void Serie_To_SerieDto_Should_Calculate_All_Statistics()
    {
        var serie = Serie.Create(Guid.NewGuid(), "Breaking Bad", new DateOnly(2008, 1, 20));
        var season1 = Season.Create(1, new DateOnly(2008, 1, 20), serie.Id);
        var ep1 = Episode.Create(1, "Ep1", TimeSpan.FromMinutes(45), season1.Id);
        ep1.MarkAsWatched();
        var ep2 = Episode.Create(2, "Ep2", TimeSpan.FromMinutes(45), season1.Id);
        season1.AddEpisode(ep1);
        season1.AddEpisode(ep2);
        serie.AddSeason(season1);

        var dto = serie.ToDto();

        Assert.Equal("Breaking Bad", dto.Title);
        Assert.Equal(1, dto.SeasonsCount);
        Assert.Equal(2, dto.TotalEpisodesCount);
        Assert.Equal(1, dto.WatchedEpisodesCount);
        Assert.Equal(50.00, dto.ProgressPercentage);
    }

    [Fact]
    public void Serie_To_SerieDto_With_No_Episodes_Should_Return_Zero_Progress()
    {
        var serie = Serie.Create(Guid.NewGuid(), "Empty Serie", new DateOnly(2024, 1, 1));

        var dto = serie.ToDto();

        Assert.Equal(0, dto.TotalEpisodesCount);
        Assert.Equal(0, dto.WatchedEpisodesCount);
        Assert.Equal(0, dto.ProgressPercentage);
    }

    [Fact]
    public void Serie_To_SerieDetailsDto_Should_Map_Seasons()
    {
        var serie = Serie.Create(Guid.NewGuid(), "Test Serie", new DateOnly(2024, 1, 1));
        serie.AddSeason(Season.Create(1, new DateOnly(2024, 1, 1), serie.Id));

        var dto = serie.ToDetailsDto();

        Assert.Single(dto.Seasons);
    }
}