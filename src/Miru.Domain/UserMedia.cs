namespace Miru.Domain
{
    public class UserMedia
    {
        private const int MIN_RATING = 0;
        private const int MAX_RATING = 5;
        /// <summary>
        /// Identifier of the user.
        /// <remarks> This identifier is a composite key based on UserId and MediaId.</remarks>
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// User associated with the media.
        /// </summary>
        public UserEntity User { get; private set; }

        /// <summary>
        /// Identifier of the media.
        /// </summary>
        public Guid MediaId { get; private set; }

        /// <summary>
        /// Media associated with the user.
        /// </summary>
        public Media Media { get; private set; }

        /// <summary>
        /// Current status of the media for the user.
        /// </summary>
        public MediaStatus Status { get; private set; }

        /// <summary>
        /// Date and time when the media was added to the user's collection.
        /// </summary>
        public DateTime AddedAt { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// Date and time when the media was watched by the user.
        /// </summary>
        public DateTime? WatchedAt { get; private set; }

        /// <summary>
        /// Rating given by the user to the media.
        /// </summary>
        public double? Rating { get; private set; }

        /// <summary>
        /// Default constructor for ORM.
        /// </summary>
        protected UserMedia() { }

        /// <summary>
        /// Constructor creating a new UserMedia.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="mediaId">Media identifier.</param>
        /// <param name="status">Status of the media.</param>
        public UserMedia(Guid userId, Guid mediaId, MediaStatus status = MediaStatus.ToWatch)
        {
            UserId = userId;
            MediaId = mediaId;
            Status = status;
        }

        /// <summary>
        /// Marks the media as watched.
        /// </summary>
        public void MarkAsWatched()
        {
            Status = MediaStatus.Watched;
            WatchedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Marks the media as watching.
        /// </summary>
        public void MarkAsWatching()
        {
            Status = MediaStatus.Watching;
            WatchedAt = null;
        }

        /// <summary>
        /// Sets the rating for the media.
        /// </summary>
        /// <param name="rating">The rating given to the media</param>
        /// <exception cref="InvalidOperationException">Thrown if the user try to rate a movie they haven't watched.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the rating is lower or higher than the given limits.</exception>
        public void SetRating(double rating)
        {
            if (Status != MediaStatus.Watched)
                throw new InvalidOperationException("Cannot rate an unwatched media.");
            if (rating < MIN_RATING || rating > MAX_RATING)
                throw new ArgumentOutOfRangeException(nameof(rating));
            Rating = rating;
        }
    }
}
