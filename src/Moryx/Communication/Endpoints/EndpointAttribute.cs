// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication.Endpoints
{
    /// <summary>
    /// Attribute for class or interface endpoint definitions
    /// </summary>
    public class EndpointAttribute : Attribute
    {
        /// <summary>
        /// Name of the endpoint
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Version of the endpoint
        /// </summary>
        public string Version { get; set; }
    }
}