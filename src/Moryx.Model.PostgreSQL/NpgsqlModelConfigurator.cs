// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moryx.Model.Configuration;
using Npgsql;

namespace Moryx.Model.PostgreSQL
{
    /// <summary>
    /// Used to configure, create and update data models
    /// </summary>
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
            // Close all connections to the server.
            // Its not possible to delete the database while there are open connections.
            NpgsqlConnection.ClearAllPools();

            // Create connection and prepare command
            var connection = new NpgsqlConnection(BuildConnectionString(config, false));
            var command = CreateCommand($"DROP DATABASE \"{config.Database}\";", connection);

            // Open connection
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }

        /// <inheritdoc />
        public override Task DumpDatabase(IDatabaseConfig config, string targetPath)
        {
            var dumpName = $"{DateTime.Now:dd-MM-yyyy-hh-mm-ss}_{config.Database}.backup";
            var fileName = Path.Combine(targetPath, dumpName);

            Logger.Log(LogLevel.Debug, "Starting to dump database with pg_dump to: {0}", fileName);

            // Create process
            var arguments = $"-U {config.Username} --format=c --file={fileName} " +
                            $"-h {config.Host} -p {config.Port} {config.Database}";

            var process = CreateBackgroundPgProcess("pg_dump.exe", arguments, config.Password);

            // Configure the process using the StartInfo properties.
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override Task RestoreDatabase(IDatabaseConfig config, string filePath)
        {
            Logger.Log(LogLevel.Debug, "Starting to restore database with pg_restore from: {0}", filePath);

            // Create process
            var arguments = $"-U {config.Username} --format=c --single-transaction --clean " +
                            $"-h {config.Host} -p {config.Port} -d {config.Database} {filePath}";

            var process = CreateBackgroundPgProcess("pg_restore.exe", arguments, config.Password);

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
            var builder = new NpgsqlConnectionStringBuilder
            {
                Username = config.Username,
                Password = config.Password,
                Host = config.Host,
                Port = config.Port,
                PersistSecurityInfo = true,
            };

            if (includeModel)
                builder.Database = config.Database;

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
    }
}
