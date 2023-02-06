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
    public class DatabaseConfig<T> : IDatabaseConfig
        where T : DatabaseConnectionSettings
    {
        /// <inheritdoc />
        [DataMember]
        public ConfigState ConfigState { get; set; }

        /// <inheritdoc />
        public string LoadError { get; set; }

        /// <inheritdoc />
        [DataMember]
        public DatabaseConnectionSettings ConnectionSettings { get; set; }

        /// <inheritdoc />
        [DataMember]
        public string ConfiguratorTypename { get; set; }

        /// <inheritdoc />
        public void Initialize()
        {
        }

        /// <inheritdoc />
        public bool IsValid()
            => !string.IsNullOrEmpty(ConfiguratorTypename)
                && ConnectionSettings.IsValid();
    }
}
