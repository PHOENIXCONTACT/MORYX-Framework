using System;
using System.Threading.Tasks;

namespace Moryx.Identity
{
    /// <summary>
    /// Authorization service to login and load a users claims/permissions
    /// </summary>
    public interface IAuthorizationService
    {
        /// <summary>
        /// Currently logged in identity
        /// </summary>
        IdentityUser CurrentIdentity { get; }

        /// <summary>
        /// Sign in identity with credentials
        /// </summary>
        Task<IdentityUser> SignInAsync(string username, string password);

        /// <summary>
        /// Sign out the current identity
        /// </summary>
        Task SignOutAsync();

        /// <summary>
        /// Event raised when the current identity has changed
        /// </summary>
        event EventHandler IdentityChanged;

        /// <summary>
        /// Event raised when an error occurred
        /// </summary>
        event EventHandler<string> ErrorOccurred;
    }
}
