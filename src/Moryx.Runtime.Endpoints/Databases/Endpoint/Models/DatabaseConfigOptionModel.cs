// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Endpoints.Databases.Endpoint.Models
{
    /// <summary>
    /// Database configurator options model
    /// </summary>
    public class DatabaseConfigOptionModel
    {
        /// <summary>
        /// Name of the configurator
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Assembly qualified name of the configurator
        /// </summary>
        public string ConfiguratorTypename { get; set; }

        /// <summary>
        /// Properties to be configured for the given configurator
        /// </summary>
        public DatabaseConfigOptionPropertyModel[] Properties { get; set; }
    }

    /// <summary>
    /// Dictionary-like field containing property information
    /// </summary>
    public class DatabaseConfigOptionPropertyModel
    {
        /// <summary>
        /// Name of the configuration option
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Default value for the option
        /// </summary>
        public string Default { get; set; }

        /// <summary>
        /// States if this option is required to be configured
        /// </summary>
        public bool Required { get; set; }
    }
}
