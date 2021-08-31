// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
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
        public TContext Create<TContext>(IDatabaseConfig config) where TContext : DbContext
        {
            var dbName = string.IsNullOrEmpty(_instanceId)
                ? Guid.NewGuid().ToString()
                : _instanceId;

            var options = new DbContextOptionsBuilder<TContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            // Create instance of context
            var context = (TContext)Activator.CreateInstance(typeof(TContext), options);
            return context;
        }
    }
}
