// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;

namespace Moryx.Runtime.Endpoints.Databases.Endpoint.Response
{
    /// <summary>
    /// Response object for testing database connections
    /// </summary>
    public class TestConnectionResponse
    {
        /// <summary>
        /// Result of the connection test
        /// </summary>
        public TestConnectionResult Result { get; set; }
    }
}
