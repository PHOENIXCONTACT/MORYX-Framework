// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moryx.Model.PostgreSQL.Attributes;

namespace Moryx.Operators.Management.Model;

/// <inheritdoc />
[NpgsqlDatabaseContext]
public class NpgsqlOperatorsContext : OperatorsContext
{
    /// <inheritdoc />
    public NpgsqlOperatorsContext()
    {
    }

    /// <inheritdoc />
    public NpgsqlOperatorsContext(DbContextOptions options) : base(options)
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
            var connectionString = configuration.GetConnectionString("Moryx.Operators.Management.Model.Npgsql");
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}

