using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using Marvin.Model;
using Marvin.Model.Npgsql;

namespace Marvin.Products.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    [DbConfigurationType(typeof(NpgsqlConfiguration))]
    [DefaultSchema(ProductsConstants.Schema)]
    public class ProductsContext : MarvinDbContext
    {
        /// <inheritdoc />
        public ProductsContext()
        { 
        }

        /// <inheritdoc />
        public ProductsContext(string connectionString, ContextMode mode) : base(connectionString, mode)
        {           
        }

        /// <inheritdoc />
        public ProductsContext(DbConnection connection, ContextMode mode) : base(connection, mode)
        {
        }

        public virtual DbSet<ProductEntity> ProductEntities { get; set; }
    
        public virtual DbSet<PartLink> PartLinks { get; set; }
    
        public virtual DbSet<RevisionHistory> RevisionHistories { get; set; }
    
        public virtual DbSet<ProductRecipeEntity> ProductRecipeEntities { get; set; }
    
        public virtual DbSet<ProductProperties> ProductProperties { get; set; }
    
        public virtual DbSet<ArticleEntity> ArticleEntities { get; set; }
    
        public virtual DbSet<WorkplanEntity> WorkplanEntities { get; set; }

        public virtual DbSet<ProductDocument> ProductDocuments { get; set; }

        public virtual DbSet<WorkplanReference> WorkplanReferences { get; set; }
    
        public virtual DbSet<StepEntity> StepEntities { get; set; }

        public virtual DbSet<ConnectorEntity> ConnectorEntities { get; set; }

        public virtual DbSet<ConnectorReference> ConnectorReferences { get; set; }
    
        public virtual DbSet<OutputDescriptionEntity> OutputDescriptionEntities { get; set; }

        /// <inheritdoc />
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Workplane reference
            modelBuilder.Entity<WorkplanReference>()
                .HasRequired(w => w.Target)
                .WithMany(w => w.TargetReferences)
                .HasForeignKey(s => s.TargetId);

            modelBuilder.Entity<WorkplanReference>()
                .HasRequired(w => w.Source)
                .WithMany(w => w.SourceReferences)
                .HasForeignKey(s => s.SourceId);

            // Article
            modelBuilder.Entity<ArticleEntity>()
                .HasRequired(p => p.Product)
                .WithMany()
                .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<ArticleEntity>()
                .HasOptional(a => a.Parent)
                .WithMany(a => a.Parts)
                .HasForeignKey(a => a.ParentId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ArticleEntity>()
                .HasOptional(a => a.PartLink)
                .WithMany()
                .HasForeignKey(a => a.PartLinkId);

            // Connector
            modelBuilder.Entity<ConnectorEntity>()
                .HasRequired(c => c.Workplan)
                .WithMany(w => w.Connectors)
                .HasForeignKey(c => c.WorkplanId);

            modelBuilder.Entity<ConnectorEntity>()
                .HasMany(c => c.Usages)
                .WithOptional(c => c.Connector)
                .HasForeignKey(c => c.ConnectorId);

            // Step entity
            modelBuilder.Entity<StepEntity>()
                .HasMany(s => s.Connectors);

            modelBuilder.Entity<StepEntity>()
                .HasMany(s => s.OutputDescriptions)
                .WithRequired(o => o.Step)
                .HasForeignKey(o => o.StepEntityId);

            modelBuilder.Entity<StepEntity>()
                .HasRequired(s => s.Workplan)
                .WithMany(w => w.Steps)
                .HasForeignKey(s => s.WorkplanId);

            modelBuilder.Entity<StepEntity>()
                .HasOptional(s => s.SubWorkplan)
                .WithMany(w => w.Parents)
                .HasForeignKey(s => s.SubWorkplanId);

            // Workplan entity
            modelBuilder.Entity<WorkplanEntity>()
                .HasMany(w => w.Recipes)
                .WithRequired(r => r.Workplan)
                .HasForeignKey(r => r.WorkplanId);

            // ProductEntity
            modelBuilder.Entity<ProductEntity>()
                .HasMany(p => p.Parts)
                .WithRequired(p => p.Parent)
                .HasForeignKey(p => p.ParentId);

            modelBuilder.Entity<ProductEntity>()
                .HasMany(p => p.Parents)
                .WithRequired(p => p.Child)
                .HasForeignKey(p => p.ChildId);

            modelBuilder.Entity<ProductEntity>()
                .HasMany(p => p.Recipes)
                .WithRequired(p => p.Product)
                .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<ProductEntity>()
                .HasMany(p => p.OldVersions)
                .WithOptional(p => p.Product)
                .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<ProductEntity>()
                .HasRequired(p => p.CurrentVersion)
                .WithMany()
                .HasForeignKey(t => t.CurrentVersionId);

            modelBuilder.Entity<ProductEntity>()
                .HasMany(p => p.Documents)
                .WithRequired(p => p.Product)
                .HasForeignKey(p => p.ProductId);

            // RevisionHistory
            modelBuilder.Entity<RevisionHistory>()
                .HasRequired(p => p.ProductRevision)
                .WithMany()
                .HasForeignKey(p => p.ProductRevisionId);

            // Indexes
            modelBuilder.Entity<ProductEntity>()
                .Property(e => e.MaterialNumber)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new[]
                    {
                        new IndexAttribute("MaterialNumber_Revision_Index", 1),
                        new IndexAttribute("Material_Number")
                    }));

            modelBuilder.Entity<ProductEntity>()
                .Property(e => e.Revision)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("MaterialNumber_Revision_Index", 2)));

            modelBuilder.Entity<ArticleEntity>()
                .Property(e => e.State)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute()));
        }
    }
}
