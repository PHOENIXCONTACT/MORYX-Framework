using System;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using Marvin.Logging;
using Npgsql;

namespace Marvin.Model.Npgsql
{
    /// <summary>
    /// Used to configure, create and update data models
    /// </summary>
    public sealed class NpgsqlModelConfigurator : ModelConfiguratorBase<NpgsqDatabaseConfig>
    {
        /// <inheritdoc />
        protected override string ProviderInvariantName => "Npgsql";

        /// <inheritdoc />
        protected override DbConnection CreateConnection(IDatabaseConfig config)
        {
            return new NpgsqlConnection(BuildConnectionString(config));
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
        public override void DeleteDatabase(IDatabaseConfig config)
        {
            // Close all connections to the server. 
            // Its not possible to delete the database while there are open connections.
            NpgsqlConnection.ClearAllPools();

            // Create connection and prepare command
            var connection = new NpgsqlConnection(BuildConnectionString(config, false));
            var command = CreateCommand($"DROP DATABASE \"{config.Database}\";", connection);

            // Open connection
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        /// <inheritdoc />
        public override void DumpDatabase(IDatabaseConfig config, string targetPath)
        {
            var dumpName = $"{DateTime.Now:dd-MM-yyyy-hh-mm-ss}_{TargetModel}_{config.Database}.backup";
            var fileName = Path.Combine(targetPath, dumpName);

            Logger.LogEntry(LogLevel.Debug, "Starting to dump database with pg_dump to: {0}", fileName);

            var process = new Process {EnableRaisingEvents = true};
            process.Exited += OnProcessCompleted;
            
            var startInfo = process.StartInfo;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
  
            startInfo.FileName = "pg_dump.exe";
            startInfo.Arguments = $"-U {config.Username} --format=c --file={fileName} -h {config.Host} -p {config.Port} {config.Database}";
            startInfo.EnvironmentVariables["PGPASSWORD"] = config.Password;

            process.OutputDataReceived += OnProcessOutputDataReceived;
            process.ErrorDataReceived += OnProcessOutputDataReceived;

            // Configure the process using the StartInfo properties.
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        /// <inheritdoc />
        public override void RestoreDatabase(IDatabaseConfig config, string filePath)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override string BuildConnectionString(IDatabaseConfig config, bool includeModel)
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
                Logger.LogEntry(LogLevel.Error, "Error while running process {0}, ExitCode: {1}!",
                    process.ProcessName, process.ExitCode);
            }
            else
            {
                Logger.LogEntry(LogLevel.Debug, "Process {0} exited successfully!",
                    process.ProcessName);
            }
        }

        private void OnProcessOutputDataReceived(object sender, DataReceivedEventArgs args)
        {
            var process = (Process)sender;
            Logger.LogEntry(LogLevel.Debug, "Process: {0}: {1}", process.ProcessName, args.Data);
        }
    }
}