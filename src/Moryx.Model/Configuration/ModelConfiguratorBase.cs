// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moryx.Configuration;

namespace Moryx.Model.Configuration
{
    /// <summary>
    /// Base class for model configurators
    /// </summary>
    public abstract class ModelConfiguratorBase<TConfig> : IModelConfigurator
        where TConfig : class, IDatabaseConfig, new()
    {
        private IConfigManager _configManager;
        private string _configName;
        private Type _contextType;

        /// <summary>
        /// Logger for this model configurator
        /// </summary>
        protected ILogger Logger { get; private set; }

        /// <inheritdoc />
        public IDatabaseConfig Config { get; private set; }

        /// <inheritdoc />
        public void Initialize(Type contextType, IConfigManager configManager, ILogger logger)
        {
            _contextType = contextType;
            _configManager = configManager;

            // Add logger
            Logger = logger;

            // Load Config
            _configName = contextType.FullName + ".DbConfig";
            Config = _configManager.GetConfiguration<TConfig>(_configName);

            // If database is empty, fill with TargetModel name
            if (string.IsNullOrWhiteSpace(Config.ConnectionSettings.Database))
                Config.ConnectionSettings.Database = contextType.Name;
        }

        /// <inheritdoc />
        public DbContext CreateContext()
        {
            return CreateContext(Config);
        }

        /// <inheritdoc />
        public DbContext CreateContext(IDatabaseConfig config)
        {
            var context = (DbContext)Activator.CreateInstance(_contextType, BuildDbContextOptions(config));
            return context;
        }

        /// <inheritdoc />
        public void UpdateConfig()
        {
            _configManager.SaveConfiguration(Config, _configName);
        }

        /// <inheritdoc />
        public virtual async Task<TestConnectionResult> TestConnection(IDatabaseConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.ConnectionSettings.Database))
                return TestConnectionResult.ConfigurationError;

            // Simple ef independent database connection
            var connectionResult = await TestDatabaseConnection(config);
            if (!connectionResult)
                return TestConnectionResult.ConnectionError;

            await using var context = CreateContext(config);

            // Ef dependent database connection
            var canConnect = await context.Database.CanConnectAsync();
            if (!canConnect)
                return TestConnectionResult.ConnectionOkDbDoesNotExist;

            // If connection is ok, test migrations
            var pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).ToArray();
            if (pendingMigrations.Any())
                return TestConnectionResult.PendingMigrations;

            return TestConnectionResult.Success;
        }

        /// <inheritdoc />
        public virtual async Task<bool> CreateDatabase(IDatabaseConfig config)
        {
            // Check is database is configured
            if (!CheckDatabaseConfig(config))
            {
                return false;
            }

            await using var context = CreateContext(config);

            //Will create the database if it does not already exist. Applies any pending migrations for the context to the database.
            await context.Database.MigrateAsync();

            // Create connection to our new database
            var connection = CreateConnection(config);
            await connection.OpenAsync();

            // Creation done -> close connection
            connection.Close();

            return true;
        }

        /// <inheritdoc />
        public virtual async Task<DatabaseMigrationSummary> MigrateDatabase(IDatabaseConfig config)
        {
            var result = new DatabaseMigrationSummary();

            await using var context = CreateContext(config);
            var pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).ToArray();

            if (pendingMigrations.Length == 0)
            {
                result.Result = MigrationResult.NoMigrationsAvailable;
                result.ExecutedMigrations = Array.Empty<string>();
                Logger.Log(LogLevel.Warning, "Database migration for database '{0}' was failed. There are no migrations available!", config.ConnectionSettings.Database);

                return result;
            }

            try
            {
                await context.Database.MigrateAsync();
                result.Result = MigrationResult.Migrated;
                result.ExecutedMigrations = pendingMigrations;
                Logger.Log(LogLevel.Information, "Database migration for database '{0}' was successful. Executed migrations: {1}",
                    config.ConnectionSettings.Database, string.Join(", ", pendingMigrations));

            }
            catch (Exception e)
            {
                result.Result = MigrationResult.Error;
                Logger.Log(LogLevel.Error, e, "Database migration for database '{0}' was failed!", config.ConnectionSettings.Database);
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<string>> AvailableMigrations(IDatabaseConfig config)
        {
            await using var context = CreateContext(config);
            try
            {
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

                return pendingMigrations.ToArray();
            }
            catch (Exception)
            {
                return Array.Empty<string>();
            }
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<string>> AppliedMigrations(IDatabaseConfig config)
        {
            await using var context = CreateContext(config);
            try
            {
                var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();

                return appliedMigrations.ToArray();
            }
            catch (Exception)
            {
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// Creates a <see cref="DbConnection"/>
        /// </summary>
        protected abstract DbConnection CreateConnection(IDatabaseConfig config);

        /// <summary>
        /// Creates a <see cref="DbConnection"/>
        /// </summary>
        protected abstract DbConnection CreateConnection(IDatabaseConfig config, bool includeModel);

        /// <summary>
        /// Creates a <see cref="DbCommand"/>
        /// </summary>
        protected abstract DbCommand CreateCommand(string cmdText, DbConnection connection);

        /// <summary>
        /// Builds options to access the database
        /// </summary>
        public abstract DbContextOptions BuildDbContextOptions(IDatabaseConfig config);

        /// <inheritdoc />
        public abstract Task DeleteDatabase(IDatabaseConfig config);

        /// <inheritdoc />
        public abstract Task DumpDatabase(IDatabaseConfig config, string targetPath);

        /// <inheritdoc />
        public abstract Task RestoreDatabase(IDatabaseConfig config, string filePath);


        /// <summary>
        /// Generally tests the connection to the database
        /// </summary>
        private async Task<bool> TestDatabaseConnection(IDatabaseConfig config)
        {
            if (!CheckDatabaseConfig(config))
                return false;

            using var conn = CreateConnection(config, false);
            try
            {
                await conn.OpenAsync();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Validates the config gor Host, Database, Username and Port
        /// </summary>
        protected static bool CheckDatabaseConfig(IDatabaseConfig config)
        {
            return (!(string.IsNullOrEmpty(config.ConfiguratorTypename) ||
                     string.IsNullOrEmpty(config.ConnectionSettings.ConnectionString)));
        }
    }
}
