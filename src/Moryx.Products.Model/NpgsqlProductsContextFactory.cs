// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Moryx.Products.Model;

public class NpgsqlProductsContextFactory : IDesignTimeDbContextFactory<NpgsqlProductsContext>
{
    public NpgsqlProductsContextFactory()
    {
        // A parameter-less constructor is required by the EF Core CLI tools.
    }

    public NpgsqlProductsContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("EFCORETOOLSDB");
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("The connection string was not set " +
                                                "in the 'EFCORETOOLSDB' environment variable.");

        var options = new DbContextOptionsBuilder<NpgsqlProductsContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new NpgsqlProductsContext(options);
    }
}