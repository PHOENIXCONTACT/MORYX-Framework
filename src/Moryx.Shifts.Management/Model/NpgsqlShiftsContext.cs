// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moryx.Model.PostgreSQL.Attributes;
using System.IO;

namespace Moryx.Shifts.Management.Model
{
    [NpgsqlDatabaseContext]
    public class NpgsqlShiftsContext : ShiftsContext
    {
        public NpgsqlShiftsContext()
        {
        }

        public NpgsqlShiftsContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                var connectionString = configuration.GetConnectionString("Moryx.Shifts.Management.Model.Npgsql");
                optionsBuilder.UseNpgsql(connectionString);
            }
        }
    }
}

