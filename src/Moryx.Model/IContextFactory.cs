// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.Configuration;

namespace Moryx.Model;

/// <summary>
/// Dedicated factory for a database context
/// </summary>
public interface IContextFactory<out TContext>
    where TContext : DbContext
{
    /// <summary>
    /// Create context using standard mode and config from config manager
    /// </summary>
    TContext Create();

    /// <summary>
    /// Create context using standard mode and alternative config
    /// </summary>
    TContext Create(DatabaseConfig config);
}