// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model;

// ReSharper disable UnusedMember.Global
// ReSharper disable VirtualMemberNeverOverridden.Global
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Moryx.Resources.Management.Model
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
