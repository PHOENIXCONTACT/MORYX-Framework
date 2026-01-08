// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;

namespace Moryx.Runtime.Endpoints.Databases.Response
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
