// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moryx.Model.PostgreSQL.Attributes;

namespace Moryx.Products.Model
{
    /// <summary>
    /// Npgsql specific implementation of <see cref="ProductsContext"/>
    /// </summary>
    [NpgsqlDatabaseContext]
    public class NpgsqlProductsContext : ProductsContext
    {
        /// <inheritdoc />
        public NpgsqlProductsContext()
        {
        }

        /// <inheritdoc />
        public NpgsqlProductsContext(DbContextOptions options) : base(options)
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
                var connectionString = configuration.GetConnectionString("Moryx.Products.Model");
                optionsBuilder.UseNpgsql(connectionString);
            }
        }
    }
}
