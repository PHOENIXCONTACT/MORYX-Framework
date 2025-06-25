using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Moryx.Identity.AccessManagement.Data;
using Moryx.Identity.AccessManagement.Models;
using Moryx.Identity.Models;

namespace Moryx.Identity.AccessManagement
{
    /// <summary>
    /// Service to take care of generating and handling the authentication token
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates a new token for the provided <paramref name="user"/>
        /// </summary>
        /// <param name="user">The user for which the token should be generated</param>
        /// <returns>The authentication result including the authentication token</returns>
        Task<AuthResult> GenerateToken(MoryxUser user);

        /// <summary>
        /// Verifies the provided token and generates a refresh token
        /// </summary>
        /// <param name="tokenRequest">The token to be verified</param>
        /// <returns>The authentication result including the refresh token</returns>
        Task<AuthResult> VerifyAndGenerateRefreshToken(TokenRequest tokenRequest);

        /// <summary>
        /// Deletes the current refresh token associated with the provided JWT.
        /// </summary>
        /// <param name="token">The JWT whose refresh token should be deleted.</param>
        Task InvalidateRefreshToken(string token);

        /// <summary>
        /// Deletes the refresh tokens for the provided user.
        /// </summary>
        /// <param name="user">The user which tokens should be deleted.</param>
        Task InvalidateRefreshTokens(MoryxUser user);

        /// <summary>
        /// Determins if the given <paramref name="token"/> is a valid authentication token
        /// </summary>
        /// <param name="token">The token to be verified</param>
        /// <returns>True if the token is a valid authentication token; false otherwise</returns>
        bool IsTokenValid(string token);

        /// <summary>
        /// Returns all claims which are assigned to at least one of the given <paramref name="roles"/>.
        /// </summary>
        /// <param name="roles">All roles to be taken into account in the enumeration</param>
        Task<IEnumerable<Claim>> GetAllPermissionClaims(IList<string> roles);
    }
}
