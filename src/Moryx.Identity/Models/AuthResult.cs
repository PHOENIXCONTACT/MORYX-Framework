// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;

namespace Moryx.Identity.Models
{
    /// <summary>
    /// Provides the result of an authentication request.
    /// </summary>
    public class AuthResult
    {
        /// <summary>
        /// The token provided with the request.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The refresh token provided with the request.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// True if the authentication operation was successfull; otherwise false.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// A list of errors occured during the authentication request.
        /// </summary>
        public List<string> Errors { get; set; }

        /// <summary>
        /// The domain associated with the authentication request.
        /// </summary>
        public string Domain { get; set; }
    }
}

