namespace Miru.Domain.UnitTests
{
    public class SerieTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Constructor_ShouldThrow_WhenTitleEmptyOrNull(string? title)
        {
            Assert.Throws<ArgumentException>(() => new Serie(title!, DateOnly.FromDateTime(DateTime.Now)));
        }

        [Fact]
        public void Constructor_ShouldCreateSerie_WhenParametersAreValid()
        {
            var title = "Breaking Bad";
            var releaseDate = DateOnly.FromDateTime(DateTime.Now);
            var description = "A high school chemistry teacher turned methamphetamine producer.";
            var posterUrl = "http://example.com/breakingbad.jpg";
            var serie = new Serie(title, releaseDate, description, posterUrl);
            Assert.Equal(title, serie.Title);
            Assert.Equal(releaseDate, serie.ReleaseDate);
            Assert.Equal(description, serie.Description);
            Assert.Equal(posterUrl, serie.PosterUrl);
        }

        [Fact]
        public void AddSeason_ShouldThrow_WhenSeasonIsNull()
        {
            var serie = new Serie("Breaking Bad", DateOnly.FromDateTime(DateTime.Now));
            Assert.Throws<ArgumentNullException>(() => serie.AddSeason(null!));
        }

        [Fact]
        public void AddSeason_ShouldThrow_WhenSeasonNumberAlreadyExists()
        {
            var serie = new Serie("Breaking Bad", DateOnly.FromDateTime(DateTime.Now));
            var season1 = new Season(1, DateOnly.FromDateTime(DateTime.Now), Guid.NewGuid());
            serie.AddSeason(season1);
            var duplicateSeason = new Season(1, DateOnly.FromDateTime(DateTime.Now), Guid.NewGuid());
            Assert.Throws<InvalidOperationException>(() => serie.AddSeason(duplicateSeason));
        }

        [Fact]
        public void RemoveSeason_ShouldThrow_WhenSeasonIsNull()
        {
            var serie = new Serie("Breaking Bad", DateOnly.FromDateTime(DateTime.Now));
            Assert.Throws<ArgumentNullException>(() => serie.RemoveSeason(null!));
        }

        [Fact]
        public void AddSeason_ShouldAddSeason_WhenValid()
        {
            var serie = new Serie("Breaking Bad", DateOnly.FromDateTime(DateTime.Now));
            var season1 = new Season(1, DateOnly.FromDateTime(DateTime.Now), Guid.NewGuid());
            serie.AddSeason(season1);
            Assert.Contains(season1, serie.Seasons);
        }

        [Fact]
        public void RemoveSeason_ShouldRemoveSeason_WhenValid()
        {
            var serie = new Serie("Breaking Bad", DateOnly.FromDateTime(DateTime.Now));
            var season1 = new Season(1, DateOnly.FromDateTime(DateTime.Now), Guid.NewGuid());
            serie.AddSeason(season1);
            serie.RemoveSeason(season1);
            Assert.DoesNotContain(season1, serie.Seasons);
        }
    }
}
