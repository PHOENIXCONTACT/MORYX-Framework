// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.Configuration;
using Moryx.Model.Repositories;

namespace Moryx.Model.InMemory
{
    /// <summary>
    /// In memory implementation of the <see cref="IUnitOfWorkFactory{TContext}"/>
    /// </summary>
    public sealed class InMemoryUnitOfWorkFactory<TContext> : IUnitOfWorkFactory<TContext>
        where TContext : DbContext
    {
        private readonly IUnitOfWorkFactory<TContext> _internalFactory;

        /// <summary>
        /// Creates a new instance of the <see cref="InMemoryUnitOfWorkFactory{TContext}"/>.
        /// A memory persistent factory will be created without any instance id.
        /// </summary>
        public InMemoryUnitOfWorkFactory() : this(string.Empty)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="InMemoryUnitOfWorkFactory{TContext}"/>.
        /// A memory persistent factory will be created with the given instance id.
        /// </summary>
        public InMemoryUnitOfWorkFactory(string instanceId)
        {
            _internalFactory = new UnitOfWorkFactory<TContext>(new InMemoryDbContextManager(instanceId));
        }

        /// <inheritdoc />
        public IUnitOfWork<TContext> Create() =>
            _internalFactory.Create();

        /// <inheritdoc />
        public IUnitOfWork<TContext> Create(IDatabaseConfig config) =>
            _internalFactory.Create(config);
    }
}