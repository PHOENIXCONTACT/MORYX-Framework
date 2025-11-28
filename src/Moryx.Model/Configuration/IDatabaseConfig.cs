// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Configuration;

namespace Moryx.Model.Configuration
{
    /// <summary>
    /// Interface for database configuration
    /// </summary>
    [DataContract]
    public abstract class DatabaseConfig : ConfigBase
    {
        /// <summary>
        /// Connection string
        /// </summary>
        [DataMember]
        public DatabaseConnectionSettings ConnectionSettings { get; set; }

        /// <summary>
        /// Database configurator typename
        /// </summary>
        [DataMember]
        public string ConfiguratorTypename { get; set; }

        /// <summary>
        /// Checks if the configuration is valid
        /// </summary>
        public abstract bool IsValid();
    }
}
