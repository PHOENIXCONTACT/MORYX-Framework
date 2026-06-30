// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moryx.Container;
using Moryx.Model.Repositories;

namespace Moryx.Model;

/// <summary>
/// Extension to activate database access in the local container
/// </summary>
public static class DbContextContainerExtension
{
    extension(IContainer container)
    {
        /// <summary>
        /// Register <see cref="IDbContextManager"/> and <see cref="IContextFactory{TContext}"/>
        /// </summary>
        public IContainer ActivateDbContexts(IDbContextManager contextManager)
        {
            container.SetInstance(contextManager);

            container.Register(typeof(ContextFactory<>), [typeof(IContextFactory<>)], "GenericContextFactory", LifeCycle.Singleton);
            container.Register(typeof(UnitOfWorkFactory<>), [typeof(IUnitOfWorkFactory<>)], "UnitOfWorkFactory", LifeCycle.Singleton);

            return container;
        }
    }

    extension(IServiceProvider serviceProvider)
    {
        /// <summary>
        /// Tries to create the database for each context.
        /// Applies missing migrations as well.
        /// </summary>
        public async Task CreateAllConfiguredDatabasesAsync(ILogger logger = null, CancellationToken token = default)
        {
            var dbContextManager = serviceProvider.GetRequiredService<IDbContextManager>();
            logger ??= serviceProvider.GetService<ILogger>();
            await dbContextManager.CreateAllConfiguredDatabasesAsync(logger, token);
        }

        /// <summary>
        /// Removes all databases
        /// </summary>
        public async Task DeleteAllConfiguredDatabasesAsync(ILogger logger = null, CancellationToken token = default)
        {
            var dbContextManager = serviceProvider.GetRequiredService<IDbContextManager>();
            logger ??= serviceProvider.GetService<ILogger>();
            foreach (var context in dbContextManager.Contexts)
            {
                token.ThrowIfCancellationRequested();
                await dbContextManager.DeleteDatabaseInternalAsync(context, logger, token);
            }
        }

        /// <summary>
        /// Deletes the database for the given context type
        /// </summary>
        public async Task DeleteDatabaseAsync(Type context, ILogger logger = null, CancellationToken token = default)
        {

            var dbContextManager = serviceProvider.GetRequiredService<IDbContextManager>();
            logger ??= serviceProvider.GetService<ILogger>();
            await dbContextManager.DeleteDatabaseInternalAsync(context, logger, token);
        }

        /// <summary>
        /// Deletes the database for the context with the given name
        /// </summary>
        public async Task DeleteDatabaseByNameAsync(string name, ILogger logger = null, CancellationToken token = default)
        {

            var dbContextManager = serviceProvider.GetRequiredService<IDbContextManager>();
            logger ??= serviceProvider.GetService<ILogger>();
            await dbContextManager.DeleteDatabaseByNameAsync(name, logger, token);
        }
    }

    extension(IDbContextManager dbContextManager)
    {
        /// <summary>
        /// Tries to create the database for each context.
        /// Applies missing migrations as well.
        /// </summary>
        public async Task CreateAllConfiguredDatabasesAsync(ILogger logger, CancellationToken token = default)
        {
            foreach (var context in dbContextManager.Contexts)
            {
                token.ThrowIfCancellationRequested();
                var configurator = dbContextManager.GetConfigurator(context);
                var testResult = await configurator.TestConnectionAsync(configurator.Config, token);

                if (testResult is TestConnectionResult.ConnectionOkDbDoesNotExist)
                {
                    logger?.LogInformation("Db for {db} does not exist. Trying to create", context.Name);
                    var createResult = await configurator.CreateDatabaseAsync(configurator.Config, token);
                    if (createResult)
                    {
                        logger?.LogInformation("Successfully created database for context {context}", context.Name);
                    }
                    else
                    {
                        logger?.LogError("Failed to create db for context {context}", context.Name);
                    }
                }
                else if (testResult is TestConnectionResult.Success)
                {
                    logger?.LogDebug("Skipping context {context} because db already exists", context.Name);
                }
                else if (testResult is TestConnectionResult.PendingMigrations)
                {
                    var summary = await configurator.MigrateDatabaseAsync(configurator.Config, token);
                    if (summary.Result is Configuration.MigrationResult.Error)
                    {
                        logger?.LogError("Failed to apply missing migration to context {context}", context.Name);
                    }
                }
                else if (testResult is TestConnectionResult.ConfigurationError or TestConnectionResult.ConnectionError)
                {
                    logger?.LogError("Skipping context {context}, because the configuration is in state {result}", context.Name, testResult);
                }
            }
        }

        /// <summary>
        /// Removes all databases
        /// </summary>
        public async Task DeleteAllConfiguredDatabasesAsync(ILogger logger, CancellationToken token = default)
        {
            foreach (var context in dbContextManager.Contexts)
            {
                token.ThrowIfCancellationRequested();
                await DeleteDatabaseInternalAsync(dbContextManager, context, logger, token);
            }
        }

        /// <summary>
        /// Deletes the database for the given context type
        /// </summary>
        /// <param name="context">Type of the context. Used to find the db to delete</param>
        /// <param name="logger">optional logger</param>
        /// <param name="token">Token to cancel the operation</param>
        /// <returns></returns>
        public async Task DeleteDatabaseAsync(Type context, ILogger logger, CancellationToken token = default)
        {

            await DeleteDatabaseInternalAsync(dbContextManager, context, logger, token);
        }

        /// <summary>
        /// Deletes the database for the context with the given name
        /// </summary>
        /// <param name="name">Name of the context. Used to find the db to delete</param>
        /// <param name="logger">optional logger</param>
        /// <param name="token">Token to cancel the operation</param>
        /// <returns></returns>
        public async Task DeleteDatabaseByNameAsync(string name, ILogger? logger, CancellationToken token = default)
        {
            var context = dbContextManager.Contexts.FirstOrDefault(context =>
                context.Name == name || context.FullName == name);
            if (context is not null)
            {
                await DeleteDatabaseInternalAsync(dbContextManager, context, logger, token);
            }
            else
            {
                logger?.LogWarning("No context with the name {name} found", name);
            }
        }

        private async Task DeleteDatabaseInternalAsync(Type context, ILogger logger, CancellationToken token = default)
        {
            var configurator = dbContextManager.GetConfigurator(context);
            var testResult = await configurator.TestConnectionAsync(configurator.Config, token);
            if (testResult is TestConnectionResult.Success or TestConnectionResult.PendingMigrations)
            {
                await configurator.DeleteDatabaseAsync(configurator.Config, token);
                var result = await configurator.TestConnectionAsync(configurator.Config, token);
                if (result is TestConnectionResult.ConnectionOkDbDoesNotExist)
                {
                    logger?.LogInformation("Successfully deleted database for context {context}", context.Name);
                }
                else
                {
                    logger?.LogError("Failed to delete db for context {context}", context.Name);
                }
            }
            else
            {
                logger?.LogError("Database connection for context {context} has problem {result}. Skipping deletion", context.Name, testResult);
            }
        }
    }
}
