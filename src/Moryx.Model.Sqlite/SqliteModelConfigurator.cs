// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moryx.Model.Configuration;
using Moryx.Modules;

namespace Moryx.Model.Sqlite
{
    /// <summary>
    /// Used to configure, create and update data models
    /// </summary>
    [DisplayName("SQLite Connector")]
    [ExpectedConfig(typeof(SqliteDatabaseConfig))]
    public sealed class SqliteModelConfigurator : ModelConfiguratorBase<SqliteDatabaseConfig>
    {
        /// <inheritdoc />
        protected override DbConnection CreateConnection(DatabaseConfig config)
        {
            return CreateConnection(config, true);
        }

        /// <inheritdoc />
        protected override DbConnection CreateConnection(DatabaseConfig config, bool includeModel)
        {
            return new SqliteConnection(config.ConnectionString);
        }

        /// <inheritdoc />
        protected override DbCommand CreateCommand(string cmdText, DbConnection connection)
        {
            return new SqliteCommand(cmdText, (SqliteConnection)connection);
        }

        /// <inheritdoc />
        public override Task DeleteDatabaseAsync(DatabaseConfig config, CancellationToken cancellationToken = default)
        {
            SqliteConnection.ClearAllPools();

            var dbFilePath = GetFilePath(config);
            if (File.Exists(dbFilePath))
                File.Delete(dbFilePath);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override DbContextOptions BuildDbContextOptions(DatabaseConfig config)
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseSqlite(config.ConnectionString);

            return builder.Options;
        }

        /// <inheritdoc />
        public override Task<TestConnectionResult> TestConnectionAsync(DatabaseConfig config, CancellationToken cancellationToken = default)
        {
            var dbFilePath = GetFilePath(config);
            var directory = Path.GetDirectoryName(dbFilePath);

            if (Directory.Exists(directory))
            {
                var result = File.Exists(dbFilePath)
                    ? TestConnectionResult.Success
                    : TestConnectionResult.ConnectionOkDbDoesNotExist;

                return Task.FromResult(result);
            }

            Directory.CreateDirectory(directory);
            return Task.FromResult(TestConnectionResult.ConnectionOkDbDoesNotExist);
        }

        private static string GetFilePath(DatabaseConfig config)
        {
            var builder = new SqliteConnectionStringBuilder(config.ConnectionString);
            return builder.DataSource;
        }

        /// <inheritdoc />
        public override Task<bool> CreateDatabaseAsync(DatabaseConfig config, CancellationToken cancellationToken = default)
        {
            if (!CheckDatabaseConfig(config))
            {
                return Task.FromResult(false);
            }

            // Overwrite the connection mode to ensure that the database
            // file can be created
            var connectionStringBuilder = new SqliteConnectionStringBuilder(config.ConnectionString)
            {
                Mode = SqliteOpenMode.ReadWriteCreate
            };
            config.ConnectionString = connectionStringBuilder.ConnectionString;

            return base.CreateDatabaseAsync(config, cancellationToken);
        }

        /// <inheritdoc />
        protected override DbContext CreateMigrationContext(DatabaseConfig config)
        {
            var migrationAssemblyType = FindMigrationAssemblyType(typeof(SqliteDbContextAttribute));

            var builder = new DbContextOptionsBuilder();
            builder.UseSqlite(config.ConnectionString, x => x.MigrationsAssembly(migrationAssemblyType.Assembly.FullName));

            return CreateContext(migrationAssemblyType, builder.Options);
        }
    }
}
