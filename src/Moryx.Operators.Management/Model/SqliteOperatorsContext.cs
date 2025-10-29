// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moryx.Model.Sqlite;

namespace Moryx.Operators.Management.Model;

/// <inheritdoc />
[SqliteDbContext(typeof(OperatorsContext))]
public class SqliteOperatorsContext : OperatorsContext
{
    /// <inheritdoc />
    public SqliteOperatorsContext()
    {
    }

    /// <inheritdoc />
    public SqliteOperatorsContext(DbContextOptions options) : base(options)
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
            var connectionString = configuration.GetConnectionString("Moryx.Operators.Management.Model.Sqlite");
            optionsBuilder.UseSqlite(connectionString);
        }
    }
}

