// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.Sqlite;

namespace Moryx.Resources.Management.Model;

/// <summary>
/// Sqlite specific implementation of <see cref="ResourcesContext"/>
/// </summary>
[SqliteDbContext(typeof(ResourcesContext))]
public class SqliteResourcesContext : ResourcesContext
{
    /// <inheritdoc />
    public SqliteResourcesContext()
    {
    }

    /// <inheritdoc />
    public SqliteResourcesContext(DbContextOptions options) : base(options)
    {
    }
}