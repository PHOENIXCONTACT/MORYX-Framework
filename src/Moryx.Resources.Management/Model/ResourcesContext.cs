// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moryx.Model;
using Moryx.Model.Attributes;
using Moryx.Model.PostgreSQL;

// ReSharper disable once CheckNamespace
namespace Moryx.Resources.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    public class ResourcesContext : MoryxDbContext
    {
        /// <inheritdoc />
        public ResourcesContext()
        {
        }

        /// <inheritdoc />
        public ResourcesContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<ResourceEntity> Resources { get; set; }

        public virtual DbSet<ResourceRelationEntity> ResourceRelations { get; set; }

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

            optionsBuilder.UseLazyLoadingProxies();
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ResourceEntity>()
                .HasMany(r => r.Sources)
                .WithOne(r => r.Target).IsRequired();

            modelBuilder.Entity<ResourceEntity>()
                .HasIndex(b => b.Name);

            modelBuilder.Entity<ResourceEntity>()
                .HasMany(r => r.Targets)
                .WithOne(r => r.Source).IsRequired();
        }
    }
}
