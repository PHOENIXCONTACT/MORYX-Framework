// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Data.Sqlite;
using Moryx.Model.Attributes;

namespace Moryx.Model.Sqlite;

/// <summary>
/// Database config for the Sqlite databases
/// </summary>
[DataContract]
public class SqliteDatabaseConfig : DatabaseConfig
{
    /// <inheritdoc />
    public override string ConfiguratorType => typeof(SqliteModelConfigurator).AssemblyQualifiedName;

    /// <summary>
    /// Path to the database file
    /// </summary>
    [Required]
    [DefaultValue("./db/<DatabaseName>.db")]
    [Description("Path to the database file")]
    [ConnectionStringKey("Data Source")]
    public string DataSource { get; set; }

    /// <summary>
    /// Connection mode that will be used when opening a connection
    /// </summary>
    [DefaultValue(SqliteOpenMode.ReadWrite)]
    [Description("Connection mode that will be used when opening a connection")]
    [ConnectionStringKey("Mode")]
    public SqliteOpenMode OpenMode { get; set; }

    /// <summary>
    /// Caching mode that will be used when opening a connection
    /// </summary>
    [DefaultValue(SqliteCacheMode.Default)]
    [Description("Caching mode that will be used when opening a connection")]
    [ConnectionStringKey("Cache")]
    public SqliteCacheMode CacheMode { get; set; }
}