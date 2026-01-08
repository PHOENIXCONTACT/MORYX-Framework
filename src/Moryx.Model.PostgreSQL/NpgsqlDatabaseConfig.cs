// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
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

        /// <summary>
        /// Name of the database to connect to
        /// </summary>
        [Required]
        [DefaultValue("<DatabaseName>")]
        [Description("Name of the database to connect to")]
        [ConnectionStringKey("Database")]
        public string Database { get; set; }

        /// <summary>
        /// Username used to connect to the database
        /// </summary>
        [Required]
        [DefaultValue("postgres")]
        [Description("Username used to connect to the database")]
        [ConnectionStringKey("Username")]
        public string Username { get; set; }

        /// <summary>
        /// Password used to connect to the database
        /// </summary>
        [Required, Password]
        [DefaultValue("postgres")]
        [Description("Password used to connect to the database")]
        [ConnectionStringKey("Password")]
        public string Password { get; set; }

        /// <summary>
        /// Hostname of the database server
        /// </summary>
        [Required]
        [DefaultValue("localhost")]
        [Description("Hostname of the database server")]
        [ConnectionStringKey("Host")]
        public string Host { get; set; }

        /// <summary>
        /// Port of the database server
        /// </summary>
        [Required]
        [DefaultValue(5432)]
        [Description("Port of the database server")]
        [ConnectionStringKey("Port")]
        public int Port { get; set; }
    }
}
