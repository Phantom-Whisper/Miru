using Microsoft.AspNetCore.Identity;

namespace Miru.Domain
{
    public class UserEntity : IdentityUser<Guid>
    {
        /// <summary>
        /// Date and time when the user was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// List of media associated with the user.
        /// </summary>
        public ICollection<UserMedia> UserMedias { get; set; } = [];
    }
}
