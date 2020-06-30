// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Data.Entity;
using Effort.Provider;

namespace Marvin.Model.InMemory
{
    /// <summary>
    /// Base implementation of <see cref="IUnitOfWorkFactory"/> for InMemory databases with Effort
    /// </summary>
    public abstract class InMemoryUnitOfWorkFactoryBase<TContext> : UnitOfWorkFactoryBase<TContext>
        where TContext : MarvinDbContext
    {
        private readonly string _instanceId;

        /// <inheritdoc />
        protected InMemoryUnitOfWorkFactoryBase() : this(string.Empty)
        {
        }

        /// <inheritdoc />
        protected InMemoryUnitOfWorkFactoryBase(string instanceId)
        {
            EffortProviderConfiguration.RegisterProvider();

            _instanceId = instanceId;
        }

        /// <inheritdoc />
        protected override MarvinDbContext CreateContext(Type contextType, ContextMode contextMode)
        {
            var connection = string.IsNullOrEmpty(_instanceId)
                ? Effort.DbConnectionFactory.CreatePersistent(Guid.NewGuid().ToString())
                : Effort.DbConnectionFactory.CreatePersistent(_instanceId);

            // Create instance of context
            var context =  (MarvinDbContext)Activator.CreateInstance(contextType, connection, contextMode);

            // Override initializer of MarvinDbContext: Create database if not exists
            Database.SetInitializer(new CreateDatabaseIfNotExists<TContext>());

            return context;
        }
    }
}
