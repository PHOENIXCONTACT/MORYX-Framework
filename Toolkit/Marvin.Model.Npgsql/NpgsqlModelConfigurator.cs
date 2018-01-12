using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using Marvin.Configuration;
using Npgsql;

namespace Marvin.Model.Npgsql
{
    /// <summary>
    /// Used to configure, create and update data models
    /// </summary>
    public sealed class NpgsqlModelConfigurator : ModelConfiguratorBase
    {
        /// <summary>
        /// Config manager of this ModelConfigurator
        /// </summary>
        private readonly IConfigManager _configManager;

        /// <summary>
        /// Name of the config
        /// </summary>
        private readonly string _configName;

        /// <summary>
        /// Configuration for migrations
        /// </summary>
        private readonly DbMigrationsConfiguration _migrationsConfiguration;

        /// <inheritdoc />
        public NpgsqlModelConfigurator(IUnitOfWorkFactory unitOfWorkFactory, IConfigManager configManager, DbMigrationsConfiguration migrationsConfiguration)
            : base(unitOfWorkFactory)
        {
            _configManager = configManager;
            _migrationsConfiguration = migrationsConfiguration;
            _configName = TargetModel + ".DbConfig";
            
            Config = _configManager.GetConfiguration<NpgsqDatabaseConfig>(_configName);
        }

        /// <inheritdoc />
        public override void UpdateConfig()
        {
            _configManager.SaveConfiguration(Config, _configName);
        }

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
        public override bool TestConnection(IDatabaseConfig config)
        {
            return !string.IsNullOrWhiteSpace(config.Database) && base.TestConnection(config);
        }

        /// <inheritdoc />
        public override DatabaseUpdateSummary UpdateDatabase(IDatabaseConfig config, string updateName)
        {
            var result = new DatabaseUpdateSummary();
            var localUpdateName = updateName;

            var availableUpdates = AvailableUpdates(config).ToList();
            if (string.IsNullOrEmpty(localUpdateName))
            {
                localUpdateName = availableUpdates.LastOrDefault()?.Name;
            }

            var isAvailable = availableUpdates.Any(databaseUpdateInformation => databaseUpdateInformation.Name == localUpdateName);
            if (isAvailable)
            {
                CreateDbMigrator(config).Update(localUpdateName);
                result.ExecutedUpdates = InstalledUpdates(config).Select(databaseUpdateInformation => new DatabaseUpdate
                {
                    Description = databaseUpdateInformation.Name
                }).ToArray();
                result.WasUpdated = true;
            }

            return result;
        }

        /// <inheritdoc />
        public override bool RollbackDatabase(IDatabaseConfig config)
        {
            CreateDbMigrator(config).Update("0");
            return true;
        }

        /// <inheritdoc />
        public override IEnumerable<DatabaseUpdateInformation> AvailableUpdates(IDatabaseConfig config)
        {
            return CreateDbMigrator(config).GetLocalMigrations().Select(name => new DatabaseUpdateInformation
            {
                Name = name
            });
        }

        /// <inheritdoc />
        public override IEnumerable<DatabaseUpdateInformation> InstalledUpdates(IDatabaseConfig config)
        {
            return CreateDbMigrator(config).GetDatabaseMigrations().Select(name => new DatabaseUpdateInformation
            {
                Name = name
            });
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

        private DbMigrator CreateDbMigrator(IDatabaseConfig config)
        {
            _migrationsConfiguration.TargetDatabase = new DbConnectionInfo(BuildConnectionString(config), "Npgsql");
            return new DbMigrator(_migrationsConfiguration);
        }
    }
}