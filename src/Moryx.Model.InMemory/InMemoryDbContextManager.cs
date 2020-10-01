// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Data.Entity;
using Moryx.Model.Configuration;

namespace Moryx.Model.InMemory
{
    /// <summary>
    /// In memory implementation of the <see cref="IDbContextManager"/>
    /// </summary>
    public sealed class InMemoryDbContextManager : IDbContextManager
    {
        private readonly string _instanceId;

        /// <summary>
        /// Creates a new instance of the <see cref="InMemoryDbContextManager"/>.
        /// A memory persistent factory will be created without any instance id.
        /// </summary>
        public InMemoryDbContextManager() : this(string.Empty)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="InMemoryDbContextManager"/>.
        /// A memory persistent factory will be created with the given instance id.
        /// </summary>
        public InMemoryDbContextManager(string instanceId)
        {
            _instanceId = instanceId;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<Type> Contexts => Array.Empty<Type>();

        /// <inheritdoc />
        public IModelConfigurator GetConfigurator(Type contextType)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IModelSetupExecutor GetSetupExecutor(Type contextType)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TContext Create<TContext>() where TContext : DbContext =>
            Create<TContext>(null);

        /// <inheritdoc />
        public TContext Create<TContext>(IDatabaseConfig config) where TContext : DbContext =>
            Create<TContext>(ContextMode.AllOn);

        /// <inheritdoc />
        public TContext Create<TContext>(ContextMode contextMode) where TContext : DbContext =>
            Create<TContext>(null, contextMode);

        /// <inheritdoc />
        public TContext Create<TContext>(IDatabaseConfig config, ContextMode contextMode) where TContext : DbContext
        {
            var connection = string.IsNullOrEmpty(_instanceId)
                ? Effort.DbConnectionFactory.CreatePersistent(Guid.NewGuid().ToString())
                : Effort.DbConnectionFactory.CreatePersistent(_instanceId);

            // Create instance of context
            var context = (TContext)Activator.CreateInstance(typeof(TContext), connection);

            // Override initializer of MoryxDbContext: Create database if not exists
            Database.SetInitializer(new CreateDatabaseIfNotExists<TContext>());

            return context;
        }
    }
}
