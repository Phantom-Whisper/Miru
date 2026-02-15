using Microsoft.AspNetCore.Identity;

namespace Miru.Domain
{
    public class UserEntity : IdentityUser<Guid>
    {
        /// <summary>
        /// Date and time when the user was created.
        /// </summary>
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// List of media associated with the user.
        /// </summary>
        private readonly List<Media> _media = [];
        public IReadOnlyCollection<Media> Media => _media.AsReadOnly();
    }
}
