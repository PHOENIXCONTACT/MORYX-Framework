// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Model.Sqlite
{
    /// <summary>
    /// Database config for the Npgsql databases
    /// </summary>
    [DataContract]
    public class SqliteDatabaseConfig : DatabaseConfig
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SqliteDatabaseConfig"/>
        /// </summary>
        public SqliteDatabaseConfig()
        {
            // Set default values
            Host = "models";
            Port = 1000;
            Database = "";
            Username = "sqlite";
            Password = "";
        }
    }
}
