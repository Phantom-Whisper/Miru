namespace Miru.Domain.UnitTests
{
    public class SeasonTests
    {
        [Fact]
        public void Constructor_ShouldThrow_WhenSeasonNumberIsInvalid()
        {
            var releaseDate = DateOnly.FromDateTime(DateTime.Now);
            var serieId = Guid.NewGuid();
            Assert.Throws<ArgumentOutOfRangeException>(() => new Season(0, releaseDate, serieId));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Season(-1, releaseDate, serieId));
        }

        [Fact]
        public void Constructor_ShouldCreateSeason_WhenParametersAreValid()
        {
            var seasonNumber = 1;
            var releaseDate = DateOnly.FromDateTime(DateTime.Now);
            var serieId = Guid.NewGuid();
            var season = new Season(seasonNumber, releaseDate, serieId);
            Assert.Equal(seasonNumber, season.SeasonNumber);
            Assert.Equal(releaseDate, season.ReleaseDate);
            Assert.Equal(serieId, season.SerieId);
        }

        [Fact]
        public void AddEpisode_ShouldThrow_WhenEpisodeIsNull()
        {
            var season = new Season(1, DateOnly.FromDateTime(DateTime.Now), Guid.NewGuid());
            Assert.Throws<ArgumentNullException>(() => season.AddEpisode(null!));
        }

        [Fact]
        public void AddEpisode_ShouldThrow_WhenEpisodeNumberAlreadyExists()
        {
            var season = new Season(1, DateOnly.FromDateTime(DateTime.Now), Guid.NewGuid());
            var episode1 = new Episode(1, "Pilot", TimeSpan.FromMinutes(45), Guid.NewGuid());
            season.AddEpisode(episode1);
            var duplicateEpisode = new Episode(1, "Second Episode", TimeSpan.FromMinutes(50), Guid.NewGuid());
            Assert.Throws<InvalidOperationException>(() => season.AddEpisode(duplicateEpisode));
        }

        [Fact]
        public void RemoveEpisode_ShouldThrow_WhenEpisodeIsNull()
        {
            var season = new Season(1, DateOnly.FromDateTime(DateTime.Now), Guid.NewGuid());
            Assert.Throws<ArgumentNullException>(() => season.RemoveEpisode(null!));
        }

        [Fact]
        public void AddEpisode_ShouldAddEpisode_WhenValid()
        {
            var season = new Season(1, DateOnly.FromDateTime(DateTime.Now), Guid.NewGuid());
            var episode1 = new Episode(1, "Pilot", TimeSpan.FromMinutes(45), Guid.NewGuid());
            season.AddEpisode(episode1);
            Assert.Contains(episode1, season.Episodes);
        }

        [Fact]
        public void RemoveEpisode_ShouldRemoveEpisode_WhenValid()
        {
            var season = new Season(1, DateOnly.FromDateTime(DateTime.Now), Guid.NewGuid());
            var episode1 = new Episode(1, "Pilot", TimeSpan.FromMinutes(45), Guid.NewGuid());
            season.AddEpisode(episode1);
            season.RemoveEpisode(episode1);
            Assert.DoesNotContain(episode1, season.Episodes);
        }
    }
}
