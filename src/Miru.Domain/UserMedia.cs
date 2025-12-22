namespace Miru.Domain
{
    public class UserMedia
    {
        /// <summary>
        /// Identifier of the user.
        /// <remarks> This identifier is a composite key based on UserId and MediaId.</remarks>
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// User associated with the media.
        /// </summary>
        public UserEntity User { get; set; }

        /// <summary>
        /// Identifier of the media.
        /// </summary>
        public Guid MediaId { get; set; }

        /// <summary>
        /// Media associated with the user.
        /// </summary>
        public MediaEntity Media { get; set; }

        /// <summary>
        /// Current status of the media for the user.
        /// </summary>
        public MediaStatus Status { get; set; }

        /// <summary>
        /// Date and time when the media was added to the user's collection.
        /// </summary>
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date and time when the media was watched by the user.
        /// </summary>
        public DateTime WatchedAt { get; set; }

        /// <summary>
        /// Rating given by the user to the media.
        /// </summary>
        public int Rating { get; set; }
    }
}
