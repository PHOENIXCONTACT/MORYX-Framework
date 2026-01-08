// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;

namespace Moryx.Runtime.Endpoints.Databases.Models
{
    /// <summary>
    /// Database configurator options model
    /// </summary>
    public class ModelConfiguratorModel
    {
        /// <summary>
        /// Name of the configurator
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Assembly qualified name of the configurator
        /// </summary>
        public string ConfiguratorType { get; set; }

        /// <summary>
        /// Keys for the connection string to map entries
        /// </summary>
        public IReadOnlyDictionary<string, string> ConnectionStringKeys { get; set; }

        /// <summary>
        /// Prototype of the connection string
        /// </summary>
        public string ConnectionStringPrototype { get; set; }

        /// <summary>
        /// Properties to be configured for the given configurator
        /// </summary>
        public Entry ConfigPrototype { get; set; }
    }
}
