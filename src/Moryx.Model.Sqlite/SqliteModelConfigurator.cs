// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moryx.Model.Configuration;
using Moryx.Model.Sqlite.Attributes;

namespace Moryx.Model.Sqlite
{
    /// <summary>
    /// Used to configure, create and update data models
    /// </summary>
    public sealed class SqliteModelConfigurator : ModelConfiguratorBase<SqliteDatabaseConfig>
    {
        /// <inheritdoc />
        protected override DbConnection CreateConnection(IDatabaseConfig config)
        {
            return CreateConnection(config, true);
        }

        /// <inheritdoc />
        protected override DbConnection CreateConnection(IDatabaseConfig config, bool includeModel)
        {
            return new SqliteConnection(BuildConnectionString(config));
        }

        /// <inheritdoc />
        protected override DbCommand CreateCommand(string cmdText, DbConnection connection)
        {
            return new SqliteCommand(cmdText, (SqliteConnection)connection);
        }

        /// <inheritdoc />
        public override Task DeleteDatabase(IDatabaseConfig config)
        {
            SqliteConnection.ClearAllPools();

            var dbFilePath = GetFilePath(config);
            if (File.Exists(dbFilePath))
                File.Delete(dbFilePath);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override Task DumpDatabase(IDatabaseConfig config, string targetPath)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override Task RestoreDatabase(IDatabaseConfig config, string filePath)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override DbContextOptions BuildDbContextOptions(IDatabaseConfig config)
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseSqlite(BuildConnectionString(config));
            

            return builder.Options;
        }

        private static string BuildConnectionString(IDatabaseConfig config)
        {
                return config.ConnectionSettings.ConnectionString;
        }

        /// <inheritdoc />
        public override Task<TestConnectionResult> TestConnection(IDatabaseConfig config)
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

        private static string GetFilePath(IDatabaseConfig config)
        {
            var builder = new SqliteConnectionStringBuilder(config.ConnectionSettings.ConnectionString);
            return builder.DataSource;
        }

        /// <inheritdoc />
        public override Task<bool> CreateDatabase(IDatabaseConfig config)
        {
            if (!CheckDatabaseConfig(config))
            {
                return Task.FromResult(false);
            }

            // Overwrite the connection mode to ensure that the database
            // file can be created
            var connectionStringBuilder = new SqliteConnectionStringBuilder(config.ConnectionSettings.ConnectionString)
            {
                Mode = SqliteOpenMode.ReadWriteCreate
            };
            config.ConnectionSettings.ConnectionString = connectionStringBuilder.ConnectionString;

            return base.CreateDatabase(config);
        }

        /// <inheritdoc />
        protected override DbContext CreateMigrationContext(IDatabaseConfig config)
        {
            var migrationAssemblyType = FindMigrationAssemblyType(typeof(SqliteContextAttribute));

            var builder = new DbContextOptionsBuilder();
            builder.UseSqlite(
                BuildConnectionString(config),
                x => x.MigrationsAssembly(migrationAssemblyType.Assembly.FullName));

            return CreateContext(migrationAssemblyType, builder.Options);
        }
    }
}
