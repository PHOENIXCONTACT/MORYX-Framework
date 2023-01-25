// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Configuration;
using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Moryx.Model.PostgreSQL
{
    /// <summary>
    /// Database config for the Npgsql databases
    /// </summary>
    [DataContract]
    public class NpgsqlDatabaseConfig : DatabaseConfig
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
        /// <summary>
        /// Creates a new instance of the <see cref="NpgsqlDatabaseConnectionSettings"/>
        /// </summary>
        public NpgsqlDatabaseConnectionSettings()
        {
            ConnectionString= "Username=postgres;Password=postgres;Host=localhost;Port=5432";
        }
    }
}
