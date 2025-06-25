using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Moryx.Identity.AccessManagement.Data
{
    /// <summary>
    /// A refresh token to prolong the lifetime of a short-lifed Jwt token
    /// </summary>
    public class RefreshToken
    {
        /// <summary>
        /// A session 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The token value.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The ID of the Jwt token this refresh token is mapped to.
        /// </summary>
        public string JwtId { get; set; }

        /// <summary>
        /// True, if it is in use; false otherwise. 
        /// </summary>
        /// <remarks>
        /// We don't want to generate a new Jwt token with the same refresh token.
        /// </remarks>
        public bool IsUsed { get; set; }

        /// <summary>
        /// True, if it has been revoke for security reasons; false otherwise.
        /// </summary>
        public bool IsRevoked { get; set; }

        /// <summary>
        /// Timestamp of the time the refresh token was generated.
        /// </summary>
        public DateTime AddedDate { get; set; }

        /// <summary>
        /// Refresh token is long lived it could last for months.
        /// </summary>
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// Id of the <see cref="MoryxUser"/> this refresh token is linked to.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The <see cref="MoryxUser"/> this refresh token is linked to.
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public MoryxUser User { get; set; }
    }
}