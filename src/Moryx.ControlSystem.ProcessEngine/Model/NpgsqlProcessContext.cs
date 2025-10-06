// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moryx.Model.PostgreSQL.Attributes;

namespace Moryx.ControlSystem.ProcessEngine.Model
{
    /// <summary>
    /// The Npgsql DbContext of this database model.
    /// </summary>
    [NpgsqlDatabaseContext]
    public class NpgsqlProcessContext : ProcessContext
    {
        /// <inheritdoc />
        public NpgsqlProcessContext()
        {
        }

        /// <inheritdoc />
        public NpgsqlProcessContext(DbContextOptions options) : base(options)
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
                var connectionString = configuration.GetConnectionString("Moryx.ControlSystem.ProcessEngine.Model");
                optionsBuilder.UseNpgsql(connectionString);
            }
        }
    }
}

