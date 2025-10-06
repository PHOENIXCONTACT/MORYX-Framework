// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication.Endpoints
{
    /// <summary>
    /// Information about a service to be used by a client for automatic configuration.
    /// </summary>
    public class Endpoint
    {
        /// <summary>
        /// Interface of this endpoint
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// Relative path of the service endpoint
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The URL of the service.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The service's version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// <c>True</c> if service requires authentication
        /// </summary>
        public bool RequiresAuthentication { get; set; }
    }
}
