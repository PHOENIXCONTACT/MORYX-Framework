// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moryx.Model.Configuration;
using Moryx.Model.PostgreSQL.Attributes;
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
        protected override DbConnection CreateConnection(IDatabaseConfig config)
        {
            return CreateConnection(config, true);
        }

        /// <inheritdoc />
        protected override DbConnection CreateConnection(IDatabaseConfig config, bool includeModel)
        {
            return new NpgsqlConnection(BuildConnectionString(config, includeModel));
        }

        /// <inheritdoc />
        protected override DbCommand CreateCommand(string cmdText, DbConnection connection)
        {
            return new NpgsqlCommand(cmdText, (NpgsqlConnection)connection);
        }

        /// <inheritdoc />
        public override async Task DeleteDatabase(IDatabaseConfig config)
        {
            var settings = (NpgsqlDatabaseConnectionSettings)config.ConnectionSettings;

            // Close all connections to the server.
            // Its not possible to delete the database while there are open connections.
            NpgsqlConnection.ClearAllPools();

            // Create connection and prepare command
            var connection = new NpgsqlConnection(BuildConnectionString(config, false));
            var command = CreateCommand($"DROP DATABASE \"{settings.Database}\";", connection);

            // Open connection
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }

        /// <inheritdoc />
        public override Task DumpDatabase(IDatabaseConfig config, string targetPath)
        {
            var connectionString = CreateConnectionStringBuilder(config);

            var dumpName = $"{DateTime.Now:dd-MM-yyyy-hh-mm-ss}_{connectionString.Database}.backup";
            var fileName = Path.Combine(targetPath, dumpName);

            Logger.Log(LogLevel.Debug, "Starting to dump database with pg_dump to: {0}", fileName);

            // Create process
            var arguments = $"-U {connectionString.Username} --format=c --file={fileName} " +
                            $"-h {connectionString.Host} -p {connectionString.Port} {connectionString.Database}";

            var process = CreateBackgroundPgProcess("pg_dump.exe", arguments, connectionString.Password);

            // Configure the process using the StartInfo properties.
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return Task.CompletedTask;
        }

        private static NpgsqlConnectionStringBuilder CreateConnectionStringBuilder(IDatabaseConfig config, bool includeModel = true)
        {
            var builder = new NpgsqlConnectionStringBuilder(config.ConnectionSettings.ConnectionString);
            
            if(includeModel)
            {
                builder.Database = config.ConnectionSettings.Database;
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
        private static IDatabaseConfig CreateTestDatabaseConfig(IDatabaseConfig config)
        {
            var testConfig = new DatabaseConfig<DatabaseConnectionSettings>
            {
                ConfiguratorTypename = config.ConfiguratorTypename,
                ConnectionSettings = config.ConnectionSettings,
                ConfigState = config.ConfigState,
                LoadError = config.LoadError
            };

            var builder = new NpgsqlConnectionStringBuilder(testConfig.ConnectionSettings.ConnectionString)
            {
                Database = "postgres"
            };

            testConfig.ConnectionSettings.ConnectionString = builder.ConnectionString;

            return testConfig;
        }

        /// <inheritdoc/>
        public override Task<TestConnectionResult> TestConnection(IDatabaseConfig config)
        {
            // Using the "postgres" database to check the server's availability.
            // might lead to edge-case when "postgres" database has been deleted.
            return base.TestConnection(CreateTestDatabaseConfig(config));
        }

        /// <inheritdoc />
        public override Task RestoreDatabase(IDatabaseConfig config, string filePath)
        {
            Logger.Log(LogLevel.Debug, "Starting to restore database with pg_restore from: {0}", filePath);
            var connectionString = CreateConnectionStringBuilder(config);

            // Create process
            var arguments = $"-U {connectionString.Username} --format=c --single-transaction --clean " +
                            $"-h {connectionString.Host} -p {connectionString.Port} -d {connectionString.Database} {filePath}";

            var process = CreateBackgroundPgProcess("pg_restore.exe", arguments, connectionString.Password);

            // Configure the process using the StartInfo properties.
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override DbContextOptions BuildDbContextOptions(IDatabaseConfig config)
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseNpgsql(BuildConnectionString(config, true));

            return builder.Options;
        }

        private static string BuildConnectionString(IDatabaseConfig config, bool includeModel)
        {
            var builder = CreateConnectionStringBuilder(config, includeModel);
            builder.PersistSecurityInfo = true;

            return builder.ToString();
        }

        private Process CreateBackgroundPgProcess(string fileName, string arguments, string pgPassword)
        {
            var process = new Process { EnableRaisingEvents = true };
            process.Exited += OnProcessCompleted;

            var startInfo = process.StartInfo;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            process.StartInfo.EnvironmentVariables["PGPASSWORD"] = pgPassword;

            startInfo.FileName = fileName;
            startInfo.Arguments = arguments;

            process.OutputDataReceived += OnProcessOutputDataReceived;
            process.ErrorDataReceived += OnProcessOutputDataReceived;

            return process;
        }

        private void OnProcessCompleted(object sender, EventArgs eventArgs)
        {
            var process = (Process)sender;
            process.Exited -= OnProcessCompleted;

            process.CancelOutputRead();
            process.CancelErrorRead();
            process.OutputDataReceived -= OnProcessOutputDataReceived;
            process.ErrorDataReceived -= OnProcessOutputDataReceived;

            if (process.ExitCode != 0)
            {
                Logger.Log(LogLevel.Error, "Error while running process {0}, ExitCode: {1}!",
                    process.Id, process.ExitCode);
            }
            else
            {
                Logger.Log(LogLevel.Debug, "Process {0} exited successfully!",
                    process.Id);
            }
        }

        private void OnProcessOutputDataReceived(object sender, DataReceivedEventArgs args)
        {
            var process = (Process)sender;
            Logger.Log(LogLevel.Debug, "Process: {0}: {1}", process.Id, args.Data);
        }

        /// <inheritdoc />
        protected override DbContext CreateMigrationContext(IDatabaseConfig config)
        {
            var migrationAssemblyType = FindMigrationAssemblyType(typeof(NpgsqlDatabaseContextAttribute));

            var builder = new DbContextOptionsBuilder();
            builder.UseNpgsql(
                BuildConnectionString(config, true),
                x => x.MigrationsAssembly(migrationAssemblyType.Assembly.FullName));

            return CreateContext(migrationAssemblyType, builder.Options);
        }
    }
}
