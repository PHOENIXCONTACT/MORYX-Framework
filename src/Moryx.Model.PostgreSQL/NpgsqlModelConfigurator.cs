// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Moryx.Model.Configuration;
using Npgsql;

namespace Moryx.Model.PostgreSQL
{
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

            // Create connection and prepare command
            var connection = new NpgsqlConnection(BuildConnectionString(config, false));
            var command = CreateCommand($"DROP DATABASE \"{settings.Database}\";", connection);

            // Open connection
            await connection.OpenAsync(cancellationToken);
            await command.ExecuteNonQueryAsync(cancellationToken);
            await connection.CloseAsync();
        }

        private static NpgsqlConnectionStringBuilder CreateConnectionStringBuilder(DatabaseConfig config, bool includeModel = true)
        {
            var builder = new NpgsqlConnectionStringBuilder(config.ConnectionString);

            if (!includeModel)
            {
                builder.Database = string.Empty;
            }

            return builder;
        }

        /// <summary>
        /// Replaces given config's database with "postgres".
        /// This config can be used to check availability of
        /// the database server.
        /// </summary>
        /// <param name="config"></param>
        /// <returns>Modified copy of given config</returns>
        private static DatabaseConfig CreateTestDatabaseConfig(DatabaseConfig config)
        {
            var testConfig = new NpgsqlDatabaseConfig
            {
                ConnectionString = config.ConnectionString,
                ConfigState = config.ConfigState,
                LoadError = config.LoadError
            };

            var builder = new NpgsqlConnectionStringBuilder(testConfig.ConnectionString)
            {
                Database = "postgres"
            };

            testConfig.ConnectionString = builder.ConnectionString;

            return testConfig;
        }

        /// <inheritdoc/>
        public override Task<TestConnectionResult> TestConnectionAsync(DatabaseConfig config, CancellationToken cancellationToken = default)
        {
            // Using the "postgres" database to check the server's availability.
            // might lead to edge-case when "postgres" database has been deleted.
            return base.TestConnectionAsync(CreateTestDatabaseConfig(config), cancellationToken);
        }

        /// <inheritdoc />
        public override DbContextOptions BuildDbContextOptions(DatabaseConfig config)
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseNpgsql(BuildConnectionString(config, true));

            return builder.Options;
        }

        private static string BuildConnectionString(DatabaseConfig config, bool includeModel)
        {
            var builder = CreateConnectionStringBuilder(config, includeModel);
            builder.PersistSecurityInfo = true;

            return builder.ToString();
        }

        /// <inheritdoc />
        protected override DbContext CreateMigrationContext(DatabaseConfig config)
        {
            var migrationAssemblyType = FindMigrationAssemblyType(typeof(NpgsqlDbContextAttribute));

            var builder = new DbContextOptionsBuilder();
            builder.UseNpgsql(
                BuildConnectionString(config, true),
                x => x.MigrationsAssembly(migrationAssemblyType.Assembly.FullName));

            return CreateContext(migrationAssemblyType, builder.Options);
        }
    }
}
