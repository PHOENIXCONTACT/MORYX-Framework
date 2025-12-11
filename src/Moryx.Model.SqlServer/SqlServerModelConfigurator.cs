// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moryx.Model.Configuration;
using System.ComponentModel;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace Moryx.Model.SqlServer;

/// <summary>
/// Used to configure, create and update data models
/// </summary>
[DisplayName("SqlServer Connector")]
public sealed class SqlServerModelConfigurator : ModelConfiguratorBase<SqlServerDatabaseConfig>
{
    /// <inheritdoc />
    protected override DbConnection CreateConnection(DatabaseConfig config)
    {
        return CreateConnection(config, true);
    }

    /// <inheritdoc />
    protected override DbConnection CreateConnection(DatabaseConfig config, bool includeModel)
    {
        return new SqlConnection(BuildConnectionString(config, includeModel));
    }

    /// <inheritdoc />
    protected override DbCommand CreateCommand(string cmdText, DbConnection connection)
    {
        return new SqlCommand(cmdText, (SqlConnection)connection);
    }

    /// <inheritdoc />
    public override async Task DeleteDatabaseAsync(DatabaseConfig config)
    {
        var settings = (SqlServerDatabaseConnectionSettings)config.ConnectionSettings;

        // Create connection and prepare command
        await using var connection = new SqlConnection(BuildConnectionString(config, false));

        var sqlCommandText = $"ALTER DATABASE {settings.Database} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;" +
                             $"DROP DATABASE [{settings.Database}]";

        await using var command = CreateCommand(sqlCommandText, connection);

        // Open connection
        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    private static SqlConnectionStringBuilder CreateConnectionStringBuilder(DatabaseConfig config, bool includeModel = true)
    {
        var builder = new SqlConnectionStringBuilder(config.ConnectionSettings.ConnectionString)
        {
            InitialCatalog = includeModel ? config.ConnectionSettings.Database : string.Empty
        };

        return builder;
    }

    /// <inheritdoc />
    public override DbContextOptions BuildDbContextOptions(DatabaseConfig config)
    {
        var builder = new DbContextOptionsBuilder();
        builder.UseSqlServer(BuildConnectionString(config, true));

        return builder.Options;
    }

    private static string BuildConnectionString(DatabaseConfig config, bool includeModel)
    {
        if (!IsValidDatabaseName(config.ConnectionSettings.Database))
            throw new ArgumentException("Invalid database name.");

        var builder = CreateConnectionStringBuilder(config, includeModel);
        builder.PersistSecurityInfo = true;

        return builder.ToString();
    }

    /// <inheritdoc />
    protected override DbContext CreateMigrationContext(DatabaseConfig config)
    {
        var migrationAssemblyType = FindMigrationAssemblyType(typeof(SqlServerDbContextAttribute));

        var builder = new DbContextOptionsBuilder();
        builder.UseSqlServer(
            BuildConnectionString(config, true),
            x => x.MigrationsAssembly(migrationAssemblyType.Assembly.FullName));

        return CreateContext(migrationAssemblyType, builder.Options);
    }

    private static bool IsValidDatabaseName(string dbName)
    {
        // Avoid sql injection by validating the database name
        if (string.IsNullOrWhiteSpace(dbName) || dbName.Length > 128)
            return false;

        // Only allow letters, numbers, and underscores
        return Regex.IsMatch(dbName, @"^[A-Za-z0-9_]+$");
    }
}
