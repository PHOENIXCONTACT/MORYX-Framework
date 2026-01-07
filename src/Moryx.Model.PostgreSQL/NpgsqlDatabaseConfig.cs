// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.Model.Attributes;

namespace Moryx.Model.PostgreSQL
{
    /// <summary>
    /// Database config for the Npgsql databases
    /// </summary>
    [DataContract]
    public class NpgsqlDatabaseConfig : DatabaseConfig
    {
        /// <inheritdoc />
        public override string ConfiguratorType => typeof(NpgsqlModelConfigurator).AssemblyQualifiedName;

        [Required]
        [DefaultValue("<DatabaseName>")]
        [ConnectionStringKey("Database")]
        public string Database { get; set; }

        [Required]
        [DefaultValue("postgres")]
        [ConnectionStringKey("Username")]
        public string Username { get; set; }

        [Required, Password]
        [DefaultValue("postgres")]
        [ConnectionStringKey("Password")]
        public string Password { get; set; }

        [Required]
        [DefaultValue("localhost")]
        [ConnectionStringKey("Host")]
        public string Host { get; set; }

        [Required]
        [DefaultValue(5432)]
        [ConnectionStringKey("Port")]
        public int Port { get; set; }
    }
}
