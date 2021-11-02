// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moryx.Model.Configuration;

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
            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = GetFilePath(config),
                Password = config.Password,
            };

            return builder.ToString();
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
            if (string.IsNullOrEmpty(config.Host))
                config.Host = "models";

            if (!Directory.Exists(config.Host))
                Directory.CreateDirectory(config.Host);

            return Path.Combine(config.Host, config.Database);
        }
    }
}
