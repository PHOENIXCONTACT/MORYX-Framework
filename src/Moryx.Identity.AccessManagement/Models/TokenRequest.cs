using System.ComponentModel.DataAnnotations;

namespace Moryx.Identity.AccessManagement.Models
{
    /// <summary>
    /// Transfer object to exchange tokens with corresponding refresh tokens
    /// </summary>
    public class TokenRequest
    {
        /// <summary>
        /// The token to be transfered
        /// </summary>
        [Required]
        public string Token { get; set; }

        /// <summary>
        /// The corresponding refresh token
        /// </summary>
        [Required]
        public string RefreshToken { get; set; }
    }
}