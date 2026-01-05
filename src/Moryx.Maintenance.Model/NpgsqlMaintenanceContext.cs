// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moryx.Model.PostgreSQL.Attributes;

namespace Moryx.Maintenance.Model;

/// <inheritdoc />
[NpgsqlDatabaseContext]
public class NpgsqlMaintenanceContext : MaintenanceContext
{
    /// <inheritdoc />
    public NpgsqlMaintenanceContext()
    {
    }

    /// <inheritdoc />
    public NpgsqlMaintenanceContext(DbContextOptions options) : base(options)
    {
    }

    /// <inheritdoc />
    public NpgsqlMaintenanceContext(DbContextOptions<NpgsqlMaintenanceContext> options) : base(options)
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
            var connectionString = configuration.GetConnectionString("Moryx.Maintenance.Management.Model.Npgsql");
            optionsBuilder.UseNpgsql(connectionString);
        }
    }

}
