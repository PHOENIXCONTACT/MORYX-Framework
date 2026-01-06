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
        where TConfig : DatabaseConfig, new()
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
        public DatabaseConfig Config { get; private set; }

        /// <inheritdoc />
        public void Initialize(Type contextType, DatabaseConfig config, ILogger logger)
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
        public DbContext CreateContext(DatabaseConfig config)
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
        public virtual async Task<TestConnectionResult> TestConnectionAsync(DatabaseConfig config, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(config.ConnectionSettings.Database))
                return TestConnectionResult.ConfigurationError;

            // Simple ef independent database connection
            var connectionResult = await TestDatabaseConnection(config, cancellationToken);
            if (!connectionResult)
                return TestConnectionResult.ConnectionError;

            await using var context = CreateContext(config);

            // Ef dependent database connection
            var canConnect = await context.Database.CanConnectAsync(cancellationToken);
            if (!canConnect)
                return TestConnectionResult.ConnectionOkDbDoesNotExist;

            // If connection is ok, test migrations
            var pendingMigrations = (await context.Database.GetPendingMigrationsAsync(cancellationToken: cancellationToken)).ToArray();
            if (pendingMigrations.Any())
                return TestConnectionResult.PendingMigrations;

            return TestConnectionResult.Success;
        }

        /// <inheritdoc />
        public virtual async Task<bool> CreateDatabaseAsync(DatabaseConfig config, CancellationToken cancellationToken = default)
        {
            // Check is database is configured
            if (!CheckDatabaseConfig(config))
            {
                return false;
            }

            await using var context = CreateMigrationContext(config);

            try
            {
                return await CreateDatabaseAsync(config, context, cancellationToken);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Creating database '{0}' failed!", config.ConnectionSettings.Database);
                return false;
            }
        }

        /// <summary>
        /// Creates a database for the given context and checks if it's possible
        /// to connect to it
        /// </summary>
        /// <param name="config">Config for testing the connection</param>
        /// <param name="context">Database context</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns></returns>
        protected async Task<bool> CreateDatabaseAsync(DatabaseConfig config, DbContext context, CancellationToken cancellationToken)
        {
            //Will create the database if it does not already exist. Applies any pending migrations for the context to the database.
            await context.Database.MigrateAsync(cancellationToken: cancellationToken);

            // Create connection to our new database
            var connection = CreateConnection(config);
            await connection.OpenAsync(cancellationToken);

            // Creation done -> close connection
            await connection.CloseAsync();

            return true;
        }

        /// <inheritdoc />
        public virtual async Task<DatabaseMigrationSummary> MigrateDatabaseAsync(DatabaseConfig config, CancellationToken cancellationToken = default)
        {
            var result = new DatabaseMigrationSummary();

            await using var context = CreateMigrationContext(config);
            var pendingMigrations = (await context.Database.GetPendingMigrationsAsync(cancellationToken: cancellationToken)).ToArray();

            if (pendingMigrations.Length == 0)
            {
                result.Result = MigrationResult.NoMigrationsAvailable;
                result.ExecutedMigrations = [];
                Logger.Log(LogLevel.Warning, "Database migration for database '{0}' failed. There are no migrations available!",
                    config.ConnectionSettings.Database);

                return result;
            }

            try
            {
                await context.Database.MigrateAsync(cancellationToken: cancellationToken);
                result.Result = MigrationResult.Migrated;
                result.ExecutedMigrations = pendingMigrations;
                Logger.Log(LogLevel.Information, "Database migration for database '{0}' was successful. Executed migrations: {1}",
                    config.ConnectionSettings.Database, string.Join(", ", pendingMigrations));
            }
            catch (Exception e)
            {
                result.Result = MigrationResult.Error;
                result.Errors = [.. result.Errors, e.Message];
                Logger.Log(LogLevel.Error, e, "Database migration for database '{0}' failed!", config.ConnectionSettings.Database);
            }

            return result;
        }

        /// <inheritdoc />
        public virtual async Task<IReadOnlyList<string>> AvailableMigrationsAsync(DatabaseConfig config, CancellationToken cancellationToken = default)
        {
            await using var context = CreateMigrationContext(config);
            return await AvailableMigrationsAsync(context, cancellationToken);
        }

        /// <summary>
        /// Retrieves all names of available updates
        /// </summary>
        /// <returns></returns>
        protected async Task<IReadOnlyList<string>> AvailableMigrationsAsync(DbContext context, CancellationToken cancellationToken)
        {
            try
            {
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync(cancellationToken);
                return pendingMigrations.ToArray();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Could not retrieve available database migrations for context '{0}'!", context.GetType().Name);
                return Array.Empty<string>();
            }
        }

        /// <inheritdoc />
        public virtual async Task<IReadOnlyList<string>> AppliedMigrationsAsync(DatabaseConfig config, CancellationToken cancellationToken = default)
        {
            await using var context = CreateMigrationContext(config);
            return await AppliedMigrationsAsync(context, cancellationToken);
        }

        /// <summary>
        /// Retrieves all names of installed updates
        /// </summary>
        /// <returns></returns>
        protected async Task<IReadOnlyList<string>> AppliedMigrationsAsync(DbContext context, CancellationToken cancellationToken)
        {
            try
            {
                var appliedMigrations = await context.Database.GetAppliedMigrationsAsync(cancellationToken);
                return appliedMigrations.ToArray();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Could not retrieve applied database migrations for context '{0}'!", context.GetType().Name);
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// Creates a <see cref="DbConnection"/>
        /// </summary>
        protected abstract DbConnection CreateConnection(DatabaseConfig config);

        /// <summary>
        /// Creates a <see cref="DbConnection"/>
        /// </summary>
        protected abstract DbConnection CreateConnection(DatabaseConfig config, bool includeModel);

        /// <summary>
        /// Creates a <see cref="DbCommand"/>
        /// </summary>
        protected abstract DbCommand CreateCommand(string cmdText, DbConnection connection);

        /// <summary>
        /// Builds options to access the database
        /// </summary>
        public abstract DbContextOptions BuildDbContextOptions(DatabaseConfig config);

        /// <inheritdoc />
        public abstract Task DeleteDatabaseAsync(DatabaseConfig config, CancellationToken cancellationToken = default);

        /// <summary>
        /// Generally tests the connection to the database
        /// </summary>
        private async Task<bool> TestDatabaseConnection(DatabaseConfig config, CancellationToken cancellationToken)
        {
            if (!CheckDatabaseConfig(config))
                return false;

            using var conn = CreateConnection(config, false);
            try
            {
                await conn.OpenAsync(cancellationToken);
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
        protected static bool CheckDatabaseConfig(DatabaseConfig config)
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
        protected virtual DbContext CreateMigrationContext(DatabaseConfig config)
        {
            return CreateContext(config);
        }
    }
}
