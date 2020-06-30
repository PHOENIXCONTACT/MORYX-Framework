// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Data.Common;
using System.Data.Entity;
using Moryx.Model;
using Moryx.Model.PostgreSQL;

// ReSharper disable once CheckNamespace
namespace Marvin.Resources.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    [DbConfigurationType(typeof(NpgsqlConfiguration))]
    [DefaultSchema(ResourcesConstants.Schema)]
    public class ResourcesContext : MarvinDbContext
    {
        /// <inheritdoc />
        public ResourcesContext()
        {
        }

        /// <inheritdoc />
        public ResourcesContext(string connectionString, ContextMode mode) : base(connectionString, mode)
        {
        }

        /// <inheritdoc />
        public ResourcesContext(DbConnection connection, ContextMode mode) : base(connection, mode)
        {
        }

        public virtual DbSet<ResourceEntity> ResourceEntities { get; set; }

        public virtual DbSet<ResourceRelation> ResourceRelations { get; set; }

        /// <inheritdoc />
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ResourceEntity>()
                .HasMany(r => r.Sources)
                .WithRequired(r => r.Target);

            modelBuilder.Entity<ResourceEntity>()
                .HasMany(r => r.Targets)
                .WithRequired(r => r.Source);
        }
    }
}
