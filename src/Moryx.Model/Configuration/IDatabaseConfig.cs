// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Configuration;

namespace Moryx.Model.Configuration
{
    /// <summary>
    /// Interface for database configuration
    /// </summary>
    public interface IDatabaseConfig : IConfig
    {
        /// <summary>
        /// Databse server
        /// </summary>
        string Host { get; set; }

        /// <summary>
        /// Port to access the server
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// Database to use
        /// </summary>
        string Database { get; set; }

        /// <summary>
        /// Databse user
        /// </summary>
        string Username { get; set; }

        /// <summary>
        /// Password for <see cref="Username"/>
        /// </summary>
        string Password { get; set; }
    }
}
