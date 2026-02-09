// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Moryx.Model.Configuration;
using Npgsql;

namespace Moryx.Model.PostgreSQL;

/// <summary>
/// Used to configure, create and update data models
/// </summary>
[DisplayName("PostgreSQL Connector")]
public sealed class NpgsqlModelConfigurator : ModelConfiguratorBase<NpgsqlDatabaseConfig>
{
    /// <inheritdoc />
    protected override DbConnection CreateConnection(DatabaseConfig config)
    {
        return CreateConnection(config, true);
    }

    /// <inheritdoc />
    protected override DbConnection CreateConnection(DatabaseConfig config, bool includeModel)
    {
        return new NpgsqlConnection(BuildConnectionString(config, includeModel));
    }

    /// <inheritdoc />
    protected override DbCommand CreateCommand(string cmdText, DbConnection connection)
    {
        return new NpgsqlCommand(cmdText, (NpgsqlConnection)connection);
    }

    /// <inheritdoc />
    public override async Task DeleteDatabaseAsync(DatabaseConfig config, CancellationToken cancellationToken = default)
    {
        var settings = (NpgsqlDatabaseConfig)config;

        // Close all connections to the server.
        // Its not possible to delete the database while there are open connections.
        NpgsqlConnection.ClearAllPools();

        // Create connection and prepare command.
        // Connect to default 'postgres' database you can't drop a database while connected to it
        var connection = new NpgsqlConnection(BuildConnectionString(config, false));
        var command = CreateCommand($"DROP DATABASE \"{settings.Database}\";", connection);

        // Open connection
        await connection.OpenAsync(cancellationToken);
        await command.ExecuteNonQueryAsync(cancellationToken);
        await connection.CloseAsync();
    }

    /// <inheritdoc />
    public override DbContextOptions BuildDbContextOptions(DatabaseConfig config)
    {
        var builder = new DbContextOptionsBuilder();
        builder.UseNpgsql(BuildConnectionString(config, true));

        return builder.Options;
    }

    /// <summary>
    /// Builds a connection string for PostgreSQL database connections.
    /// </summary>
    /// <param name="config">The database configuration containing the connection string.</param>
    /// <param name="includeModel">If true, uses the configured database; if false, connects to the default 'postgres' database.</param>
    /// <returns>The connection string for PostgreSQL.</returns>
    private static string BuildConnectionString(DatabaseConfig config, bool includeModel)
    {
        var builder = new NpgsqlConnectionStringBuilder(config.ConnectionString);

        if (!includeModel)
        {
            builder.Database = "postgres";
        }

        builder.PersistSecurityInfo = true;

        return builder.ToString();
    }

    /// <inheritdoc />
    protected override DbContext CreateMigrationContext(DatabaseConfig config)
    {
        var migrationAssemblyType = FindMigrationAssemblyType(typeof(NpgsqlDbContextAttribute));

        var builder = new DbContextOptionsBuilder();
        builder.UseNpgsql(BuildConnectionString(config, true),
            x => x.MigrationsAssembly(migrationAssemblyType.Assembly.FullName));

        return CreateContext(migrationAssemblyType, builder.Options);
    }
}
