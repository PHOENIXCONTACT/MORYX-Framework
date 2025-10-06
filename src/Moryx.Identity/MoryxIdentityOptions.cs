// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Authentication;

namespace Moryx.Identity
{
    /// <summary>
    /// Options class provides information needed to control MORYX Identity Authentication handler behavior
    /// </summary>
    public class MoryxIdentityOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Name to be used to configure the MoryxIdentity authentication scheme
        /// </summary>
        public const string MoryxIdentity = "MoryxIdentity";

        /// <summary>
        /// Gets or sets the url at which user permissions are requested remotely
        /// </summary>
        public string BaseAddress { get; set; }

        /// <summary>
        /// Gets the cookie name under which the JWT token for the remote request can be found 
        /// </summary>
        [Obsolete("Use " + nameof(MoryxIdentityDefaults.JWT_COOKIE_NAME) + " instead, if you want to retrieve the name of the cookie.")]
        public string CookieName { get; } = "moryx_user_token";

        /// <summary>
        /// Gets the url to which the request is posted
        /// </summary>
        public string RequestUri { get; } = "/api/auth/user/permissions";

        /// <summary>
        /// Uri to retrieve a new token using a refresh token
        /// </summary>
        public string RefreshTokenUri { get; } = "/api/auth/RefreshToken";
    }
}

