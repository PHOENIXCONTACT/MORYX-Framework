// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
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

        public IReadOnlyDictionary<string, string> ConnectionStringKeys { get; set; }

        /// <summary>
        /// Properties to be configured for the given configurator
        /// </summary>
        public Entry ConfigPrototype { get; set; }
    }
}
