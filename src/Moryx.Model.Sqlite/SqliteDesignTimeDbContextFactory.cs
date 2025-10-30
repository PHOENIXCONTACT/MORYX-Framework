// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;

namespace Moryx.Model.Sqlite;

/// <summary>
/// Base class for design time factories for Sqlite DbContexts
/// </summary>
/// <typeparam name="TContext">Type of the DbContext</typeparam>
public abstract class SqliteDesignTimeDbContextFactory<TContext> : DesignTimeDbContextFactory<TContext>
    where TContext : DbContext
{
    /// <inheritdoc />
    protected override TContext CreateDbContext(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TContext>();
        optionsBuilder.UseSqlite(connectionString);

        return (TContext)Activator.CreateInstance(typeof(TContext), optionsBuilder.Options)!;
    }
}
