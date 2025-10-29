// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moryx.Model.PostgreSQL;

namespace Moryx.Orders.Management.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    [NpgsqlDbContext(typeof(OrdersContext))]
    public class NpgsqlOrdersContext : OrdersContext
    {
        /// <inheritdoc />
        public NpgsqlOrdersContext()
        {
        }

        /// <inheritdoc />
        public NpgsqlOrdersContext(DbContextOptions options) : base(options)
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
                var connectionString = configuration.GetConnectionString("Moryx.Orders.Management.Model");
                optionsBuilder.UseNpgsql(connectionString);
            }
        }
    }
}

