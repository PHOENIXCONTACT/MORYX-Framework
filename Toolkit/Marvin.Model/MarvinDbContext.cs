using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Marvin.Model
{
    /// <summary>
    /// Marvin related implementation of <see cref="T:System.Data.Entity.DbContext" />
    /// </summary>
    [DefaultSchema("public")]
    public abstract class MarvinDbContext : DbContext, IContextMode
    {
        /// <inheritdoc />
        public ContextMode CurrentMode { get; private set; }

        /// <summary>
        /// Default constructor, only need for migration tools
        /// </summary>
        protected MarvinDbContext()
        {

        }

        /// <summary>
        /// Initializes a new Entities object using the connection string found in the 'Entities' section of the application configuration file.
        /// </summary>
        protected MarvinDbContext(string connectionString, ContextMode mode) : base(connectionString)
        {
            Configure(mode);
        }

        /// <summary>
        /// Initializes a new Entities object using an exisiting connection.
        /// </summary>
        protected MarvinDbContext(DbConnection connection, ContextMode mode) : base(connection, true)
        {
            Configure(mode);
        }

        /// <inheritdoc />
        public void Configure(ContextMode mode)
        {
            Configuration.ProxyCreationEnabled = mode.HasFlag(ContextMode.ProxyOnly);
            Configuration.LazyLoadingEnabled = mode.HasFlag(ContextMode.LazyLoading);
            Configuration.AutoDetectChangesEnabled = mode.HasFlag(ContextMode.ChangeTracking);
            Configuration.ValidateOnSaveEnabled = true;
            CurrentMode = mode;
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
            var defaultSchemaAttr = GetType().GetCustomAttribute<DefaultSchemaAttribute>();
            if (!string.IsNullOrEmpty(defaultSchemaAttr?.Schema))
                modelBuilder.HasDefaultSchema(defaultSchemaAttr.Schema.ToLower());

            // Custon Code-First Conventions: https://msdn.microsoft.com/en-us/library/jj819164(v=vs.113).aspx
            // Turn off pluralization
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        /// <summary>
        /// Configure properties
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void ConfigureProperties(DbModelBuilder modelBuilder)
        {
            // Column with name "Id" will be the primary key
            modelBuilder.Properties().Where(p => p.Name == "Id").Configure(p => p.IsKey());

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
            SetTimestamps();
            return base.SaveChanges();
        }

        /// <inheritdoc />
        public override Task<int> SaveChangesAsync()
        {
            SetTimestamps();
            return base.SaveChangesAsync();
        }

        /// <inheritdoc />
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            SetTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void SetTimestamps()
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
    }
}