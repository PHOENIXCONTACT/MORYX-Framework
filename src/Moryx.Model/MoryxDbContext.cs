// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Moryx.Model
{
    /// <summary>
    /// Moryx related implementation of <see cref="T:System.Data.Entity.DbContext" />
    /// </summary>
    public abstract class MoryxDbContext : DbContext
    {
        private static readonly MethodInfo DbInitializerMethod;

        /// <summary>
        /// Static constructor to load the database initializer method
        /// </summary>
        static MoryxDbContext()
        {
            DbInitializerMethod = (from method in typeof(Database).GetMethods()
                where method.Name.Equals(nameof(Database.SetInitializer)) &&
                      method.GetGenericArguments().Length == 1
                select method).FirstOrDefault();
        }

        /// <summary>
        /// Constructor for this DbContext.
        /// Used by EntityFramework migration tools.
        /// </summary>
        protected MoryxDbContext()
        {
        }

        /// <summary>
        /// Constructor for this DbContext using the given connection string.
        /// Used by the Runtime environment
        /// </summary>
        protected MoryxDbContext(string connectionString) : base(connectionString)
        {
            SetNullDatabaseInitializer();
        }

        /// <summary>
        /// Constructor for this DbContext using an existing connection.
        /// Used if connection is already defined (e.g. Effort InMemory)
        /// </summary>
        protected MoryxDbContext(DbConnection connection) : base(connection, true)
        {
            SetNullDatabaseInitializer();
        }

        /// <inheritdoc />
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            ConfigureConventions(modelBuilder);
            ConfigureProperties(modelBuilder);
        }

        /// <summary>
        /// Configure EntityFramework conventions
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void ConfigureConventions(DbModelBuilder modelBuilder)
        {
            // Set default schmema
            var attributes = GetType().GetCustomAttributes<DefaultSchemaAttribute>().ToArray();
            var defaultSchemaAttr = attributes.LastOrDefault();
            modelBuilder.HasDefaultSchema(!string.IsNullOrEmpty(defaultSchemaAttr?.Schema)
                ? defaultSchemaAttr.Schema.ToLower() // schema names have to be lower case!
                : DefaultSchemaAttribute.DefaultName);

            // Custom Code-First Conventions: https://msdn.microsoft.com/en-us/library/jj819164(v=vs.113).aspx
            // Turn off pluralization
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        /// <summary>
        /// Configure properties
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void ConfigureProperties(DbModelBuilder modelBuilder)
        {
            // Properties flagged with IsUnicode will be configured
            modelBuilder.Properties()
                .Having(x => x.GetCustomAttributes(false).OfType<IsUnicodeAttribute>().FirstOrDefault())
                .Configure((config, att) => config.IsUnicode(att.Unicode));

            // Type of string
            modelBuilder.Properties<string>().Configure(p => p.IsMaxLength());
        }

        /// <inheritdoc />
        public override int SaveChanges()
        {
            ApplyModificationTracking();
            return base.SaveChanges();
        }

        /// <inheritdoc />
        public override Task<int> SaveChangesAsync()
        {
            ApplyModificationTracking();
            return base.SaveChangesAsync();
        }

        /// <inheritdoc />
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            ApplyModificationTracking();
            return base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Applies the modification tracking and updates the Created, Updated and Deleted columns
        /// </summary>
        private void ApplyModificationTracking()
        {
            var modifiedTrackedEntries = from entry in ChangeTracker.Entries()
                where entry.Entity is IModificationTrackedEntity &&
                      (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                select new
                {
                    Entry = entry,
                    Entity = entry.Entity as IModificationTrackedEntity,
                    entry.State,
                };

            var changeTimestamp = DateTime.UtcNow;
            foreach (var modified in modifiedTrackedEntries)
            {
                modified.Entity.Updated = changeTimestamp;

                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (modified.State == EntityState.Added)
                {
                    modified.Entity.Created = changeTimestamp;
                }
                else if (modified.State == EntityState.Modified)
                {
                    if (modified.Entry.CurrentValues[nameof(IModificationTrackedEntity.Deleted)] !=
                        modified.Entry.OriginalValues[nameof(IModificationTrackedEntity.Deleted)])
                        modified.Entity.Deleted = changeTimestamp;
                }
            }
        }

        /// <summary>
        /// Sets the <see cref="NullDatabaseInitializer{TContext}"/> to not create the database automatically
        /// </summary>
        private void SetNullDatabaseInitializer()
        {
            // Because of the limited entity framdework API, we have to call the Database initializer by reflection
            // Database.SetInitializer(new NullDatabaseInitializer<CustomContext>());
            var contextType = GetType();
            var initializerMethod = DbInitializerMethod.MakeGenericMethod(contextType);

            var nullInitializerType = typeof(NullDatabaseInitializer<>).MakeGenericType(contextType);
            var nullInitializer = Activator.CreateInstance(nullInitializerType);

            initializerMethod.Invoke(null, new[] { nullInitializer });
        }
    }
}
