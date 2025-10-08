// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Data.Sqlite;

namespace Moryx.Model.Sqlite
{
    /// <summary>
    /// Database config for the Sqlite databases
    /// </summary>
    [DataContract]
    public class SqliteDatabaseConfig : DatabaseConfig<SqliteDatabaseConnectionSettings>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SqliteDatabaseConfig"/>
        /// </summary>
        public SqliteDatabaseConfig()
        {
            ConnectionSettings = new SqliteDatabaseConnectionSettings();
            ConfiguratorTypename = typeof(SqliteModelConfigurator).AssemblyQualifiedName;
        }
    }

    /// <summary>
    /// Database connection settings for the Sqlite databases
    /// </summary>
    public class SqliteDatabaseConnectionSettings : DatabaseConnectionSettings
    {
        private string _database;

        /// <summary>
        /// Default constructor of <see cref="SqliteDatabaseConnectionSettings"/>
        /// </summary>
        public SqliteDatabaseConnectionSettings()
        {
            var defaultDbPath = Path.Combine(".", "db", "<DatabaseName>.db");
            ConnectionString = $"Data Source={defaultDbPath};Mode=ReadWrite;";
        }

        /// <inheritdoc />
        [DataMember]
        public override string Database
        {
            get => _database;
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                _database = value;
                ConnectionString = ConnectionString?.Replace("<DatabaseName>", value);
            }
        }

        /// <inheritdoc />
        [DataMember, Required]
        public override string ConnectionString { get; set; }

        /// <inheritdoc />
        public override bool IsValid()
        {
            try
            {
                var builder = new SqliteConnectionStringBuilder(ConnectionString);
                return !string.IsNullOrEmpty(ConnectionString);
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}
