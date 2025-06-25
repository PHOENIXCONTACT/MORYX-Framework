using System;

namespace Moryx.Identity.AccessManagement
{
    /// <summary>
    /// Provides constants for cookies handled by the AccessManagement
    /// </summary>
    [Obsolete("Use " + nameof(MoryxIdentityDefaults) + " instead.")]
    public static class Cookies
    {
        /// <summary>
        /// Name of the refresh token cookie
        /// </summary>
        [Obsolete("Use " + nameof(MoryxIdentityDefaults.REFRESH_TOKEN_COOKIE_NAME) + " instead, if you want to retrieve the name of the refresh token cookie.")]
        public const string REFRESH_TOKEN_COOKIE_NAME = "moryx_refresh_token";

        /// <summary>
        /// Name of the JWT cookie
        /// </summary>
        [Obsolete("Use " + nameof(MoryxIdentityDefaults.JWT_COOKIE_NAME) + " instead, if you want to retrieve the name of the cookie.")]
        public const string JWT_COOKIE_NAME = "moryx_user_token";

        /// <summary>
        /// Name of the user cookie
        /// </summary>
        [Obsolete("Use " + nameof(MoryxIdentityDefaults.USER_COOKIE_NAME) + " instead, if you want to retrieve the name of the user cookie.")]
        public const string USER_COOKIE_NAME = "moryx_user";
    }
}
