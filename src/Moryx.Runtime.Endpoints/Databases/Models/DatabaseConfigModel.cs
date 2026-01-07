// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;

namespace Moryx.Runtime.Endpoints.Databases.Models
{
    /// <summary>
    /// Configuration of the database.
    /// </summary>
    public record DatabaseConfigModel
    {
        /// <summary>
        /// Name of the database configurator type
        /// </summary>
        public string ConfiguratorType { get; set; }

        /// <summary>
        /// Configuration of the database model.
        /// </summary>
        public Entry Properties { get; set; }

        public string ConnectionString { get; set; }
    }
}
