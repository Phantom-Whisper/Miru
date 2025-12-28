namespace Miru.Domain.UnitTests
{
    public class EpisodeTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Constructor_NullOrEmptyTitle_ShouldThrowArgumentNullException(string? title)
        {
            Assert.Throws<ArgumentException>(() => new Episode(1, title!, TimeSpan.FromMinutes(42), Guid.NewGuid()));
        }

        [Fact]
        public void Constructor_NegativeEpisodeNumber_ShouldThrowArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Episode(-1, "Pilot", TimeSpan.FromMinutes(42), Guid.NewGuid()));
        }

        [Fact]
        public void Constructor_ZeroDuration_ShouldThrowArgumentException()
        {
            var seasonId = Guid.NewGuid();
            Assert.Throws<ArgumentException>(() => new Episode(1, "Pilot", TimeSpan.Zero, seasonId));
        }

        [Fact]
        public void Constructor_NegativeDuration_ShouldThrowArgumentException()
        {
            var seasonId = Guid.NewGuid();
            Assert.Throws<ArgumentException>(() => new Episode(1, "Pilot", TimeSpan.FromMinutes(-10), seasonId));
        }

        [Fact]
        public void Constructor_ValidParameters_ShouldCreateEpisode()
        {
            int episodeNumber = 1;
            string title = "Pilot";
            TimeSpan duration = TimeSpan.FromMinutes(42);
            Guid seasonId = Guid.NewGuid();
            var episode = new Episode(episodeNumber, title, duration, seasonId);
            Assert.Equal(episodeNumber, episode.EpisodeNumber);
            Assert.Equal(title, episode.Title);
            Assert.Equal(duration, episode.Duration);
            Assert.Equal(seasonId, episode.SeasonId);
        }
    }
}
