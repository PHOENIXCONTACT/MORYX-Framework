// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moryx.Model.Attributes;
using Moryx.Model.SqlServer;
using Moryx.TestTools.Test.Model;
using System.IO;

// ReSharper disable once CheckNamespace
namespace Moryx.Resources.Model;

/// <summary>
/// SqlServer specific implementation of <see cref="TestModelContext"/>
/// </summary>
[SqlServerDatabaseContext]
[ModelConfigurator(typeof(SqlServerModelConfigurator))]
[DefaultSchema("resources")]
public class SqlServerTestModelContext : TestModelContext
{
    /// <inheritdoc />
    public SqlServerTestModelContext()
    {
    }

    /// <inheritdoc />
    public SqlServerTestModelContext(DbContextOptions options) : base(options)
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
            var connectionString = configuration.GetConnectionString("Moryx.Resources.Model");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}