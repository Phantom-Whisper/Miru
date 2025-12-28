namespace Miru.Domain.UnitTests
{
    public class UserMediaTests
    {
        [Fact]
        public void MarkAsWatched_ShouldSetStatusAndWatchedAt()
        { 
            var userMedia = new UserMedia(Guid.NewGuid(), Guid.NewGuid());
            userMedia.MarkAsWatched();
            Assert.Equal(MediaStatus.Watched, userMedia.Status);
            Assert.NotNull(userMedia.WatchedAt);
        }

        [Fact]
        public void MarkAsWatching_ShouldSetStatusAndClearWatchedAt()
        {
            var userMedia = new UserMedia(Guid.NewGuid(), Guid.NewGuid());
            userMedia.MarkAsWatched();
            userMedia.MarkAsWatching();
            Assert.Equal(MediaStatus.Watching, userMedia.Status);
            Assert.Null(userMedia.WatchedAt);
        }

        [Fact]
        public void SetRating_ShouldThrow_WhenNotWatched()
        {
            var userMedia = new UserMedia(Guid.NewGuid(), Guid.NewGuid());
            Assert.Throws<InvalidOperationException>(() => userMedia.SetRating(4));
        }

        [Fact]
        public void SetRating_ShouldThrow_WhenRatingIsOutOfRange()
        {
            var userMedia = new UserMedia(Guid.NewGuid(), Guid.NewGuid());
            userMedia.MarkAsWatched();
            Assert.Throws<ArgumentOutOfRangeException>(() => userMedia.SetRating(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => userMedia.SetRating(6));
        }

        [Fact]
        public void SetRating_ShouldSetRating_WhenRatingIsValid()
        {
            var userMedia = new UserMedia(Guid.NewGuid(), Guid.NewGuid());
            userMedia.MarkAsWatched();
            userMedia.SetRating(4.5);
            Assert.Equal(4.5, userMedia.Rating);
        }
    }
}
