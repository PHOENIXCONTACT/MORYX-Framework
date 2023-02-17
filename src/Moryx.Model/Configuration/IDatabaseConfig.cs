// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
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
        /// Connection string
        /// </summary>
        DatabaseConnectionSettings ConnectionSettings { get; set; }

        /// <summary>
        /// Database configurator typename
        /// </summary>
        string ConfiguratorTypename { get; set; }

        /// <summary>
        /// Checks if the configuration is valid
        /// </summary>
        public bool IsValid();
    }
}
