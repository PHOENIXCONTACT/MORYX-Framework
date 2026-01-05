// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moryx.Model.Sqlite.Attributes;

namespace Moryx.Maintenance.Model;

/// <inheritdoc />
[SqliteContext]
public class SqliteMaintenanceContext : MaintenanceContext
{
    /// <inheritdoc />
    public SqliteMaintenanceContext()
    {
    }

    public SqliteMaintenanceContext(DbContextOptions options) : base(options)
    {
    }

    /// <inheritdoc />
    public SqliteMaintenanceContext(DbContextOptions<SqliteMaintenanceContext> options) : base(options)
    {
    }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = configuration.GetConnectionString("Moryx.Maintenance.Management.Model.Sqlite");
            optionsBuilder.UseSqlite(connectionString);
        }
    }

}
