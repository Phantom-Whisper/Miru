namespace Miru.Domain.UnitTests;

public class MovieTests
{
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Constructor_ShouldThrow_WhenTitleEmptyOrNull(string? title)
    {
        Assert.Throws<ArgumentException>(() => new Movie(title!, DateOnly.FromDateTime(DateTime.Now), TimeSpan.FromMinutes(90)));
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenDurationIsZero()
    {
        Assert.Throws<ArgumentException>(() => new Movie("Inception", DateOnly.FromDateTime(DateTime.Now), TimeSpan.Zero));
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenDurationIsNegative()
    {
        Assert.Throws<ArgumentException>(() => new Movie("Inception", DateOnly.FromDateTime(DateTime.Now), TimeSpan.FromMinutes(-10)));
    }

    [Fact]
    public void Constructor_ShouldCreateMovie_WhenParametersAreValid()
    {
        var title = "Inception";
        var releaseDate = DateOnly.FromDateTime(DateTime.Now);
        var duration = TimeSpan.FromMinutes(90);
        var description = "A mind-bending thriller.";
        var posterUrl = "http://example.com/inception.jpg";
        var movie = new Movie(title, releaseDate, duration, description, posterUrl);
        Assert.Equal(title, movie.Title);
        Assert.Equal(releaseDate, movie.ReleaseDate);
        Assert.Equal(duration, movie.Duration);
        Assert.Equal(description, movie.Description);
        Assert.Equal(posterUrl, movie.PosterUrl);
    }
    
    [Fact]
    public void AddUserMedia_ShouldThrow_WhenUserMediaIsNull()
    {
        var movie = new Movie("Inception", DateOnly.FromDateTime(DateTime.Now), TimeSpan.FromMinutes(90));
        Assert.Throws<ArgumentNullException>(() => movie.AddUserMedia(null!));
    }

    [Fact]
    public void RemoveUserMedia_ShouldThrow_WhenUserMediaIsNull()
    {
        var movie = new Movie("Inception", DateOnly.FromDateTime(DateTime.Now), TimeSpan.FromMinutes(90));
        Assert.Throws<ArgumentNullException>(() => movie.RemoveUserMedia(null!));
    }

    [Fact]
    public void AddUserMedia_ShouldAdduserMedia_WhenValid()
    {
        var movie = new Movie("Inception", DateOnly.FromDateTime(DateTime.Now), TimeSpan.FromMinutes(120));
        var userMedia = new UserMedia(Guid.NewGuid(), Guid.NewGuid());
        movie.AddUserMedia(userMedia);
        Assert.Contains(userMedia, movie.UserMedias);
    }

    [Fact]
    public void RemoveUserMedia_ShouldRemove_WhenValid()
    {
        var movie = new Movie("Inception", DateOnly.FromDateTime(DateTime.Now), TimeSpan.FromMinutes(120));
        var userMedia = new UserMedia(Guid.NewGuid(), Guid.NewGuid());
        movie.AddUserMedia(userMedia);
        movie.RemoveUserMedia(userMedia);
        Assert.DoesNotContain(userMedia, movie.UserMedias);

    }

}
