// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Identity
{
    /// <summary>
    /// Default values used by MORYX identity authentication.
    /// </summary>
    public static class MoryxIdentityDefaults
    {
        /// <summary>
        /// Default value for AuthenticationScheme property in the MoryxIdentityAuthenticationOptions
        /// </summary>
        public const string AuthenticationScheme = "MoryxIdentity";

        /// <summary>
        /// Name of the refresh token cookie
        /// </summary>
        public const string REFRESH_TOKEN_COOKIE_NAME = "moryx_refresh_token";

        /// <summary>
        /// Name of the JWT cookie
        /// </summary>
        public const string JWT_COOKIE_NAME = "moryx_user_token";

        /// <summary>
        /// Name of the user cookie
        /// </summary>
        public const string USER_COOKIE_NAME = "moryx_user";
    }
}

