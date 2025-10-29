// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moryx.Model;
using Moryx.Model.PostgreSQL;
using Moryx.Model.Sqlite;

namespace Moryx.TestTools.Test.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    [NpgsqlDbContext]
    [SqliteDbContext]
    public class TestModelContext : MoryxDbContext
    {
        public TestModelContext()
        {
        }

        public TestModelContext(DbContextOptions options) : base(options)
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
                var connectionString = configuration.GetConnectionString("Moryx.TestTools.Test.Model");
                optionsBuilder.UseNpgsql(connectionString);
            }

            optionsBuilder.UseLazyLoadingProxies();
        }
        public virtual DbSet<CarEntity> Cars { get; set; }

        public virtual DbSet<WheelEntity> Wheels { get; set; }

        public virtual DbSet<SportCarEntity> SportCars { get; set; }

        public virtual DbSet<JsonEntity> Jsons { get; set; }

        public virtual DbSet<HugePocoEntity> HugePocos { get; set; }

        public virtual DbSet<HouseEntity> Houses { get; set; }
    }
}
