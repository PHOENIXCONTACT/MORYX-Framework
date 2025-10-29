// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model;
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable once CheckNamespace

namespace Moryx.Resources.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    public abstract class ResourcesContext : MoryxDbContext
    {
        /// <inheritdoc />
        protected ResourcesContext()
        {
        }

        /// <inheritdoc />
        protected ResourcesContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<ResourceEntity> Resources { get; set; }

        public virtual DbSet<ResourceRelationEntity> ResourceRelations { get; set; }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

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
