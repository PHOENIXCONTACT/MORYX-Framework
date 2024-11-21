// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moryx.Model.PostgreSQL.Attributes;

// ReSharper disable once CheckNamespace
namespace Moryx.Resources.Model
{
    /// <summary>
    /// Npgsql specific implementation of <see cref="ResourcesContext"/>
    /// </summary>
    [NpgsqlDatabaseContext]
    public class NpgsqlResourcesContext : ResourcesContext
    {
        /// <inheritdoc />
        public NpgsqlResourcesContext()
        {
        }

        /// <inheritdoc />
        public NpgsqlResourcesContext(DbContextOptions options) : base(options)
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
                optionsBuilder.UseNpgsql(connectionString);
            }
        }
    }
}
