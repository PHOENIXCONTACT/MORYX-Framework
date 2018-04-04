using System;
using System.Data.Common;
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
        public override void DumpDatabase(IDatabaseConfig config, string filePath)
        {
            throw new NotImplementedException();
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
    }
}