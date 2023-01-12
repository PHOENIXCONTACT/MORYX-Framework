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
        public string Username => Regex.Matches(ConnectionString, @"Username=(\w+)")[0].Groups[0].Value;
        public string Password => Regex.Matches(ConnectionString, @"Password=(\w+)")[0].Groups[0].Value;
        public int Port => Convert.ToInt32(Regex.Matches(ConnectionString, @"Port=(\d+)")[0].Groups[0].Value);
        public string Host => Regex.Matches(ConnectionString, @"Host=(\w+)")[0].Groups[0].Value;

        /// <summary>
        /// Creates a new instance of the <see cref="NpgsqlDatabaseConnectionSettings"/>
        /// </summary>
        public NpgsqlDatabaseConnectionSettings()
        {
            ConnectionString= "Username=postgres;Password=postgres;Host=localhost;Port=5432";
        }
    }
}
