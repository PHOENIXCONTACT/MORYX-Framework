// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Data.Entity;
using Moryx.Model.Configuration;

namespace Moryx.Model
{
    /// <summary>
    /// Global context management
    /// </summary>
    public interface IDbContextManager
    {
        IReadOnlyCollection<IModelConfigurator> Configurators { get; }

        TContext Create<TContext>()
            where TContext : DbContext;

        TContext Create<TContext>(IDatabaseConfig config)
            where TContext : DbContext;

        TContext Create<TContext>(ContextMode contextMode)
            where TContext : DbContext;

        TContext Create<TContext>(IDatabaseConfig config, ContextMode contextMode)
            where TContext : DbContext;
    }
}
