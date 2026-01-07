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
    public class SqliteDatabaseConfig : DatabaseConfig
    {
        public override string ConfiguratorType => typeof(SqliteModelConfigurator).AssemblyQualifiedName;

        [DataMember, Required, DefaultValue("Data Source=<DatabaseName>;Mode=ReadWrite")]
        public override string ConnectionString { get; set; }

        [DataMember, Required]
        [ConnectionStringKey("Data Source")]
        public string DataSource { get; set; }

        [DataMember]
        [ConnectionStringKey("Mode")]
        public SqliteOpenMode OpenMode { get; set; }

        [DataMember]
        [ConnectionStringKey("Cache")]
        public SqliteCacheMode CacheMode { get; set; }
    }
}
