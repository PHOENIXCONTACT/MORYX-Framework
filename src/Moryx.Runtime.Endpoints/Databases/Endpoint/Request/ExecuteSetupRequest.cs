// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Endpoints.Databases.Endpoint.Models;

namespace Moryx.Runtime.Endpoints.Databases.Endpoint.Request
{
    /// <summary>
    /// Request to execute a database setup
    /// </summary>
    public class ExecuteSetupRequest
    {
        /// <summary>
        /// Configuration of the database
        /// </summary>
        public DatabaseConfigModel Config { get; set; }

        /// <summary>
        /// Setup to be executed
        /// </summary>
        public SetupModel Setup { get; set; }
    }
}
