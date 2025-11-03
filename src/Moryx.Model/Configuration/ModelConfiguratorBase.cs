// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moryx.Tools;

namespace Moryx.Model.Configuration
{
    /// <summary>
    /// Base class for model configurators
    /// </summary>
    public abstract class ModelConfiguratorBase<TConfig> : IModelConfigurator
        where TConfig : class, IDatabaseConfig, new()
    {
        /// <summary>
        /// The underlying context's type
        /// </summary>
        protected Type ContextType { get; private set; }

        /// <summary>
        /// Logger for this model configurator
        /// </summary>
        protected ILogger Logger { get; private set; }

        /// <inheritdoc />
        public IDatabaseConfig Config { get; private set; }

        /// <inheritdoc />
        public void Initialize(Type contextType, IDatabaseConfig config, ILogger logger)
        {
            ContextType = contextType;

            // Add logger
            Logger = logger;

            Config = config as TConfig;
            if (Config == null)
                throw new InvalidOperationException(
                    $"Configuration for model '{contextType.FullName}' is not of expected type '{typeof(TConfig).FullName}'");
        }

        /// <inheritdoc />
        public DbContext CreateContext()
        {
            return CreateContext(Config);
        }

        /// <inheritdoc />
        public DbContext CreateContext(IDatabaseConfig config)
        {
            return CreateContext(ContextType, BuildDbContextOptions(config));
        }

        /// <inheritdoc />
        public DbContext CreateContext(Type contextType, DbContextOptions dbContextOptions)
        {
            var context = (DbContext)Activator.CreateInstance(contextType, dbContextOptions);
            return context;
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

            await using var context = CreateMigrationContext(config);

            return await CreateDatabase(config, context);
        }

        /// <summary>
        /// Creates a database for the given context and checks if it's possible
        /// to connect to it
        /// </summary>
        /// <param name="config">Config for testing the connection</param>
        /// <param name="context">Database context</param>
        /// <returns></returns>
        protected async Task<bool> CreateDatabase(IDatabaseConfig config, DbContext context)
        {
            //Will create the database if it does not already exist. Applies any pending migrations for the context to the database.
            await context.Database.MigrateAsync();

            // Create connection to our new database
            var connection = CreateConnection(config);
            await connection.OpenAsync();

            // Creation done -> close connection
            await connection.CloseAsync();

            return true;
        }

        /// <inheritdoc />
        public virtual async Task<DatabaseMigrationSummary> MigrateDatabase(IDatabaseConfig config)
        {
            var result = new DatabaseMigrationSummary();

            await using var context = CreateMigrationContext(config);
            var pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).ToArray();

            if (pendingMigrations.Length == 0)
            {
                result.Result = MigrationResult.NoMigrationsAvailable;
                result.ExecutedMigrations = [];
                Logger.Log(LogLevel.Warning, "Database migration for database '{0}' was failed. There are no migrations available!",
                    config.ConnectionSettings.Database);

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
        public virtual async Task<IReadOnlyList<string>> AvailableMigrations(IDatabaseConfig config)
        {
            await using var context = CreateMigrationContext(config);
            return await AvailableMigrations(context);
        }

        /// <summary>
        /// Retrieves all names of available updates
        /// </summary>
        /// <returns></returns>
        protected async Task<IReadOnlyList<string>> AvailableMigrations(DbContext context)
        {
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
        public virtual async Task<IReadOnlyList<string>> AppliedMigrations(IDatabaseConfig config)
        {
            await using var context = CreateMigrationContext(config);
            return await AppliedMigrations(context);
        }

        /// <summary>
        /// Retrieves all names of installed updates
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyList<string>> AppliedMigrations(DbContext context)
        {
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
            catch(Exception e)
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

        /// <summary>
        /// Finds the context type marked with the provided attribute type.
        /// </summary>
        protected Type FindMigrationAssemblyType(Type attributeType)
        {
            var contextTypes =
                ReflectionTool.GetPublicClasses(ContextType);

            var fileteredAssembly = contextTypes.FirstOrDefault(t => t.CustomAttributes.Any(a => a.AttributeType == attributeType));

            return fileteredAssembly ?? contextTypes.First();
        }

        /// <summary>
        /// Creates a context for migration purposes based on a config
        /// </summary>
        protected virtual DbContext CreateMigrationContext(IDatabaseConfig config)
        {
            return CreateContext(config);
        }
    }
}
