using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Moryx.Identity.AccessManagement.Data;

namespace Moryx.Identity.AccessManagement.Models
{
    /// <summary>
    /// Maps to <see cref="MoryxUser"/>.
    /// </summary>
    public class MoryxUserUpdateModel
    {
        /// <summary>
        /// Maps to <see cref="IdentityUser{TKey}.Email"/>.
        /// </summary>
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Maps to <see cref="IdentityUser{TKey}.UserName"/>.
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Maps to <see cref="MoryxUser.Firstname"/>.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Maps to <see cref="MoryxUser.LastName"/>.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Maps to <see cref="PasswordReset.ResetToken"/>.
        /// </summary>
        [Editable(false)]
        public string PasswordResetToken { get; set; }
    }
}