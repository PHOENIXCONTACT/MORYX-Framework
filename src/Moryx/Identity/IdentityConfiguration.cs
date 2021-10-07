// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Identity
{
    /// <summary>
    /// Configuration for the identity management
    /// </summary>
    public static class IdentityConfiguration
    {
        /// <summary>
        /// Current context used for the identity management
        /// </summary>
        public static IAuthorizationContext CurrentContext { get; set; }
    }
}