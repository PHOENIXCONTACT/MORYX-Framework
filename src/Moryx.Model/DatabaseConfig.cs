// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.Model.Configuration;

namespace Moryx.Model
{
    /// <inheritdoc />
    [DataContract]
    public class DatabaseConfig<T> : DatabaseConfig
        where T : DatabaseConnectionSettings
    {
        /// <inheritdoc />
        public override bool IsValid()
            => !string.IsNullOrEmpty(ConfiguratorTypename)
                && ConnectionSettings.IsValid();
    }
}
