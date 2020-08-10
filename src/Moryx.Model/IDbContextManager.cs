// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Data.Entity;
using Moryx.Model.Configuration;

namespace Moryx.Model
{
    /// <summary>
    /// Global component for handling database contexts
    /// </summary>
    public interface IDbContextManager
    {
        /// <summary>
        /// Per database context model configurator
        /// </summary>
        IReadOnlyCollection<IModelConfigurator> Configurators { get; }

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
        TContext Create<TContext>(IDatabaseConfig config)
            where TContext : DbContext;

        /// <summary>
        /// Creates a database context with the default configuration and custom initial context model
        /// </summary>
        /// <typeparam name="TContext">Database context type</typeparam>
        /// <returns>Preconfigured instance of the given DbContext</returns>
        TContext Create<TContext>(ContextMode contextMode)
            where TContext : DbContext;

        /// <summary>
        /// Creates a database context with the given configuration and custom initial context model
        /// </summary>
        /// <typeparam name="TContext">Database context type</typeparam>
        /// <returns>Preconfigured instance of the given DbContext</returns>
        TContext Create<TContext>(IDatabaseConfig config, ContextMode contextMode)
            where TContext : DbContext;
    }
}
