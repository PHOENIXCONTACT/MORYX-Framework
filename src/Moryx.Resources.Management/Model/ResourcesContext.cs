// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Data.Common;
using System.Data.Entity;
using Moryx.Model;
using Moryx.Model.PostgreSQL;

// ReSharper disable once CheckNamespace
namespace Moryx.Resources.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    [ModelConfigurator(typeof(NpgsqlModelConfigurator))]
    [DbConfigurationType(typeof(NpgsqlConfiguration))]
    public class ResourcesContext : MoryxDbContext
    {
        /// <inheritdoc />
        public ResourcesContext()
        {
        }

        /// <inheritdoc />
        public ResourcesContext(string connectionString) : base(connectionString)
        {
        }

        /// <inheritdoc />
        public ResourcesContext(DbConnection connection) : base(connection)
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
