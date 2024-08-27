// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.Configuration;

namespace Moryx.Model
{
    /// <inheritdoc />
    public class ContextFactory<TContext> : IContextFactory<TContext>
        where TContext : DbContext
    {
        private readonly IDbContextManager _manager;

        /// <summary>
        /// Creates a new instance of <see cref="ContextFactory{TContext}"/>
        /// </summary>
        public ContextFactory(IDbContextManager dbContextManager)
        {
            _manager = dbContextManager;
        }

        /// <inheritdoc />
        public TContext Create()
        {
            return _manager.Create<TContext>();
        }

        /// <inheritdoc />
        public TContext Create(IDatabaseConfig config)
        {
            return _manager.Create<TContext>(config);
        }
    }
}