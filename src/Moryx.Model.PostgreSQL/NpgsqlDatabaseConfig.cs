// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Moryx.Model.PostgreSQL
{
    /// <summary>
    /// Database config for the Npgsql databases
    /// </summary>
    [DataContract]
    public class NpgsqlDatabaseConfig : DatabaseConfig<NpgsqlDatabaseConnectionSettings>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="NpgsqlDatabaseConfig"/>
        /// </summary>
        public NpgsqlDatabaseConfig()
        {
            ConnectionSettings = new NpgsqlDatabaseConnectionSettings();
            ConfiguratorTypename = typeof(NpgsqlModelConfigurator).AssemblyQualifiedName;
        }
    }

    /// <summary>
    /// Database connection settings for the Npgsql databases
    /// </summary>
    public class NpgsqlDatabaseConnectionSettings : DatabaseConnectionSettings
    {
        /// <inheritdoc/>
        [DataMember, Required]
        public override string Database { get; set; }

        /// <inheritdoc/>
        [DataMember, Required, DefaultValue("Username=postgres;Password=postgres;Host=localhost;Port=5432")]
        public override string ConnectionString { get; set; }
    }
}
