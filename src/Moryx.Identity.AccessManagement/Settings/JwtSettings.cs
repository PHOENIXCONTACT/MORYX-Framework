// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Identity.AccessManagement.Settings
{
    /// <summary>
    /// Settings for the creation of the JWT token in the MORYX AccessManagement
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Issuer of the JWT token
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Application secret used for the creation of JWT tokens
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Expiration Time of the refresh token in days
        /// </summary>
        public int RefreshTokenExpirationInDays { get; set; }

        /// <summary>
        /// Expiration Time of the token in minutes
        /// </summary>
        public int ExpirationInMinutes { get; set; }
    }
}

