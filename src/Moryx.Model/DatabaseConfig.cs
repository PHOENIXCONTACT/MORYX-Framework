// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.Model.Attributes;
using Moryx.Model.Configuration;
using Moryx.Serialization;

namespace Moryx.Model
{
    /// <inheritdoc />
    [DataContract]
    public class DatabaseConfig : IDatabaseConfig
    {
        /// <inheritdoc />
        [DataMember]
        public ConfigState ConfigState { get; set; }

        /// <inheritdoc />
        public string LoadError { get; set; }

        /// <inheritdoc />
        [DataMember]
        [PluginConfigs(typeof(DatabaseConnectionSettings))]
        public DatabaseConnectionSettings ConnectionSettings { get; set; }

        /// <inheritdoc />
        [DataMember]
        [PossibleConfigurators]
        public string ConfiguratorTypename { get; set; }

        public void Initialize()
        {
        }
    }
}
