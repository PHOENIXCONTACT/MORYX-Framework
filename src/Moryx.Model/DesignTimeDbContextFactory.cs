// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Moryx.Model;

/// <summary>
/// Base class for Design time factories for DbContexts
/// Evaluates the -- --connection "connectionString" argument or the EFCORETOOLSDB environment variable
/// </summary>
/// <typeparam name="TContext">Type of the DbContext</typeparam>
public abstract class DesignTimeDbContextFactory<TContext> : IDesignTimeDbContextFactory<TContext> where TContext : DbContext
{
    /// <inheritdoc />
    public virtual TContext CreateDbContext(string[] args)
    {
        var connectionString = ResolveConnectionString(args);

        return CreateDbContext(connectionString);
    }

    protected static string ResolveConnectionString(string[] args)
    {
        var connectionString = string.Empty;
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == "--connection" && i + 1 < args.Length)
            {
                connectionString = args[i + 1].Trim('"');
            }
        }

        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = Environment.GetEnvironmentVariable("EFCORETOOLSDB");
        }

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("The connection string was not set " +
                                                "in the 'EFCORETOOLSDB' environment variable nor by arguments -- --connection \"connectionString\"");
        }

        return connectionString;
    }

    /// <summary>
    /// Create context based on the given connection string
    /// </summary>
    /// <param name="connectionString">Connection string for the DbContext</param>
    protected abstract TContext CreateDbContext(string connectionString);
}
