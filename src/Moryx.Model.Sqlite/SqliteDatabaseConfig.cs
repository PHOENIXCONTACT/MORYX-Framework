// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Data.Sqlite;
using Moryx.Model.Attributes;

namespace Moryx.Model.Sqlite
{
    /// <summary>
    /// Database config for the Sqlite databases
    /// </summary>
    [DataContract]
    public class SqliteDatabaseConfig : DatabaseConfig
    {
        /// <inheritdoc />
        public override string ConfiguratorType => typeof(SqliteModelConfigurator).AssemblyQualifiedName;

        [Required, DefaultValue("./db/<DatabaseName>.db")]
        [ConnectionStringKey("Data Source")]
        public string DataSource { get; set; }

        [ConnectionStringKey("Mode"), DefaultValue(SqliteOpenMode.ReadWrite)]
        public SqliteOpenMode OpenMode { get; set; }

        [ConnectionStringKey("Cache")]
        public SqliteCacheMode CacheMode { get; set; }
    }
}
