// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;

namespace Moryx.Model
{
    /// <summary>
    /// Global component for handling database contexts
    /// </summary>
    public interface IDbContextManager
    {
        /// <summary>
        /// Found DbContext types
        /// </summary>
        IReadOnlyCollection<Type> Contexts { get; }

        /// <summary>
        /// Get configurator for the given context type
        /// </summary>
        IModelConfigurator GetConfigurator(Type contextType);

        /// <summary>
        /// Get configurator instance for the given context type
        /// </summary>
        IModelConfigurator GetConfigurator(Type contextType, Type configuratorType, DatabaseConfig databaseConfig);

        /// <summary>
        /// Get possible configurators for the given context type
        /// </summary>
        Type[] GetConfigurators(Type contextType);

        /// <summary>
        /// Get setup executor for a model
        /// </summary>
        IModelSetupExecutor GetSetupExecutor(Type contextType);

        /// <summary>
        /// Creates a database context with the default configuration
        /// </summary>
        /// <typeparam name="TContext">Database context type</typeparam>
        /// <returns>Preconfigured instance of the given DbContext</returns>
        TContext Create<TContext>()
            where TContext : DbContext;

        /// <summary>
        /// Creates a database context with the given configuration
        /// </summary>
        /// <typeparam name="TContext">Database context type</typeparam>
        /// <returns>Preconfigured instance of the given DbContext</returns>
        TContext Create<TContext>(DatabaseConfig config)
            where TContext : DbContext;

        /// <summary>
        /// Updates the database configurator for the given context type
        /// </summary>
        /// <param name="dbContextType">Database context type</param>
        /// <param name="configuratorType">Configurator type</param>
        /// <param name="dbConfig">Database config</param>
        void UpdateConfig(Type dbContextType, Type configuratorType, DatabaseConfig dbConfig);
    }
}
