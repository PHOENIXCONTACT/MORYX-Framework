using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using Marvin.Model;
using Marvin.Model.Npgsql;

namespace Marvin.Resources.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    [DbConfigurationType(typeof(NpgsqlConfiguration))]
    public class ResourcesContext : MarvinDbContext
    {
        /// <summary>
        /// Initialize a new ResourcesContext object.
        /// </summary>
        public ResourcesContext()
        {
        }

        /// <inheritdoc />
        public ResourcesContext(string connectionString, ContextMode mode) : base(connectionString, mode)
        {
        }

        /// <inheritdoc />
        public ResourcesContext(DbConnection dbConnection, ContextMode mode) : base(dbConnection, mode)
        {
        }

        /// <summary>
        /// There are no comments for ResourceEntity in the schema.
        /// </summary>
        public virtual DbSet<ResourceEntity> ResourceEntities { get; set; }

        /// <summary>
        /// There are no comments for ResourceRelation in the schema.
        /// </summary>
        public virtual DbSet<ResourceRelation> ResourceRelations { get; set; }

        /// <inheritdoc cref="MarvinDbContext.OnModelCreating"/>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // ResourceEntity
            modelBuilder.Entity<ResourceEntity>()
                .HasMany(r => r.Sources)
                .WithRequired(r => r.Target);

            modelBuilder.Entity<ResourceEntity>()
                .HasMany(r => r.Targets)
                .WithRequired(r => r.Source);

            // Indexes
            modelBuilder.Entity<ResourceEntity>()
                .Property(e => e.Name)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute()));

            modelBuilder.Entity<ResourceEntity>()
                .Property(e => e.LocalIdentifier)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute()));

            modelBuilder.Entity<ResourceEntity>()
                .Property(e => e.GlobalIdentifier)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute()));
        }
    }
}
