// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Moryx.Model
{
    /// <summary>
    /// Moryx related implementation of <see cref="T:System.Data.Entity.DbContext" />
    /// </summary>
    public abstract class MoryxDbContext : DbContext
    {
        protected string ConnectionString { get; }

        protected DbConnection Connection { get; }

        /// <summary>
        /// Static constructor to load the database initializer method
        /// </summary>
        static MoryxDbContext()
        {

        }

        protected MoryxDbContext()
        {
        }

        protected MoryxDbContext(DbConnection connection)
        {
            Connection = connection;
        }

        protected MoryxDbContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected MoryxDbContext(DbContextOptions options) : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureConventions(modelBuilder);
            ConfigureProperties(modelBuilder);
        }

        /// <summary>
        /// Configure EntityFramework conventions
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void ConfigureConventions(ModelBuilder modelBuilder)
        {
            // Set default schema
            var attributes = GetType().GetCustomAttributes<DefaultSchemaAttribute>().ToArray();
            var defaultSchemaAttr = attributes.LastOrDefault();

            modelBuilder.HasDefaultSchema(!string.IsNullOrEmpty(defaultSchemaAttr?.Schema)
                ? defaultSchemaAttr.Schema.ToLower() // schema names have to be lower case!
                : DefaultSchemaAttribute.DefaultName);

            // Custom Code-First Conventions: https://msdn.microsoft.com/en-us/library/jj819164(v=vs.113).aspx
            // Turn off pluralization
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        /// <summary>
        /// Configure properties
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void ConfigureProperties(ModelBuilder modelBuilder)
        {
            // Properties flagged with IsUnicode will be configured
            //modelBuilder.Properties()
            //    .Having(x => x.GetCustomAttributes(false).OfType<IsUnicodeAttribute>().FirstOrDefault())
            //    .Configure((config, att) => config.IsUnicode(att.Unicode));

            //// Type of string
            //modelBuilder.Properties<string>().Configure(p => p.IsMaxLength());
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
