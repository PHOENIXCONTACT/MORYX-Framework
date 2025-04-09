// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Moryx.Model.Annotations;
using Moryx.Model.Attributes;

namespace Moryx.Model
{
    /// <summary>
    /// Moryx related implementation of <see cref="T:System.Data.Entity.DbContext" />
    /// </summary>
    public abstract class MoryxDbContext : DbContext
    {
        /// <inheritdoc />
        protected MoryxDbContext()
        {
        }

        /// <inheritdoc />
        protected MoryxDbContext(DbContextOptions options) : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Utc DateTime conversion
            modelBuilder.ApplyDateTimeKindConverter();

            // Set default schema
            var attributes = GetType().GetCustomAttributes<DefaultSchemaAttribute>().ToArray();
            var defaultSchemaAttr = attributes.LastOrDefault();

            modelBuilder.HasDefaultSchema(!string.IsNullOrEmpty(defaultSchemaAttr?.Schema)
                ? defaultSchemaAttr.Schema.ToLower() // schema names have to be lower case!
                : DefaultSchemaAttribute.DefaultName);
        }

        /// <inheritdoc />
        public override int SaveChanges()
        {
            ApplyModificationTracking();
            return base.SaveChanges();
        }

        /// <inheritdoc />
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            ApplyModificationTracking();
            return base.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            ApplyModificationTracking();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// Applies the modification tracking and updates the Created, Updated and Deleted columns
        /// </summary>
        private void ApplyModificationTracking()
        {
            var modifiedTrackedEntries = ChangeTracker.Entries()
                .Where(entry => entry.Entity is IModificationTrackedEntity &&
                                (entry.State == EntityState.Added || entry.State == EntityState.Modified));

            var timeStamp = DateTime.UtcNow;
            foreach (var entry in modifiedTrackedEntries)
            {
                var entity = (IModificationTrackedEntity)entry.Entity;

                // Added gets created
                if (entry.State == EntityState.Added)
                    entity.Created = timeStamp;

                // All states gets updated
                entity.Updated = timeStamp;

                // Override deleted date of entity to sync between updated and deleted
                if (entry.State == EntityState.Modified && entry.OriginalValues[nameof(IModificationTrackedEntity.Deleted)] == null && entity.Deleted != null)
                    entity.Deleted = timeStamp;
            }
        }
    }
}
