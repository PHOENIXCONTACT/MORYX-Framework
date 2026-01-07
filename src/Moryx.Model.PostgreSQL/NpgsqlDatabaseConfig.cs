// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.Configuration;

namespace Moryx.Model.PostgreSQL
{
    /// <summary>
    /// Database config for the Npgsql databases
    /// </summary>
    [DataContract]
    public class NpgsqlDatabaseConfig : DatabaseConfig
    {
        private static readonly Dictionary<string, string> _connectionStringKeys;
        public override string ConfiguratorType => typeof(NpgsqlModelConfigurator).AssemblyQualifiedName;

        [DataMember, Required, DefaultValue("Database=<DatabaseName>;Username=postgres;Password=postgres;Host=localhost;Port=5432")]
        public override string ConnectionString { get; set; }

        [DataMember, Required]
        [DefaultValue("<DatabaseName>")]
        [ConnectionStringKey("Database")]
        public string Database { get; set; }

        [DataMember, Required]
        [DefaultValue("postgres")]
        [ConnectionStringKey("Username")]
        public string Username { get; set; }

        [DataMember, Required, Password]
        [DefaultValue("postgres")]
        [ConnectionStringKey("Password")]
        public string Password { get; set; }

        [DataMember, Required]
        [DefaultValue("localhost")]
        [ConnectionStringKey("Host")]
        public string Host { get; set; }

        [DataMember, Required]
        [DefaultValue(5432)]
        [ConnectionStringKey("Port")]
        public int Port { get; set; }
    }
}
