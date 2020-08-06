// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Data.Entity;
using Moryx.Model.Configuration;

namespace Moryx.Model
{
    /// <inheritdoc />
    public class ContextFactory<TContext> : IContextFactory<TContext>
        where TContext : DbContext
    {
        /// <summary>
        /// Injected manager that does the real work
        /// </summary>
        public IDbContextManager Manager { get; set; }

        /// <inheritdoc />
        public TContext Create()
        {
            return Manager.Create<TContext>();
        }

        /// <inheritdoc />
        public TContext Create(IDatabaseConfig config)
        {
            return Manager.Create<TContext>(config);
        }

        /// <inheritdoc />
        public TContext Create(ContextMode contextMode)
        {
            return Manager.Create<TContext>(contextMode);
        }

        /// <inheritdoc />
        public TContext Create(IDatabaseConfig config, ContextMode contextMode)
        {
            return Manager.Create<TContext>(config, contextMode);
        }
    }
}