// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Configuration;
using System.Runtime.Serialization;

namespace Moryx.Model.Sqlite
{
    /// <summary>
    /// Database config for the Sqlite databases
    /// </summary>
    [DataContract]
    public class SqliteDatabaseConfig : DatabaseConfig
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

        [DataMember]
        public override string Database
        {
            get => _database; 
            set
            {
                _database = value;
                ConnectionString = ConnectionString.Replace("<DatabaseName>", value);
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SqliteDatabaseConnectionSettings"/>
        /// </summary>
        public SqliteDatabaseConnectionSettings()
        {
            ConnectionString = $"Data Source=.\\db\\<DatabaseName>.db";
        }
    }
}
