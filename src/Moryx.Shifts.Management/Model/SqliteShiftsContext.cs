// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using Moryx.Model.Sqlite;

namespace Moryx.Shifts.Management.Model
{
    [SqliteDbContext(typeof(ShiftsContext))]
    public class SqliteShiftsContext : ShiftsContext
    {
        public SqliteShiftsContext()
        {
        }

        public SqliteShiftsContext(DbContextOptions options) : base(options)
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
                var connectionString = configuration.GetConnectionString("Moryx.Shifts.Management.Model.Sqlite");
                optionsBuilder.UseSqlite(connectionString);
            }
        }
    }
}

