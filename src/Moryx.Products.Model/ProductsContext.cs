// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using Moryx.Model;
using Moryx.Model.PostgreSQL;

namespace Moryx.Products.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    [ModelConfigurator(typeof(NpgsqlModelConfigurator))]
    [DbConfigurationType(typeof(NpgsqlConfiguration))]
    public class ProductsContext : MoryxDbContext
    {
        /// <inheritdoc />
        public ProductsContext()
        {
        }

        /// <inheritdoc />
        public ProductsContext(string connectionString) : base(connectionString)
        {
        }

        /// <inheritdoc />
        public ProductsContext(DbConnection connection) : base(connection)
        {
        }

        public virtual DbSet<ProductTypeEntity> ProductEntities { get; set; }

        public virtual DbSet<PartLink> PartLinkEntities { get; set; }

        public virtual DbSet<ProductRecipeEntity> ProductRecipeEntities { get; set; }

        public virtual DbSet<ProductProperties> ProductPropertiesEntities { get; set; }

        public virtual DbSet<ProductFileEntity> ProductFiles { get; set; }

        public virtual DbSet<ProductInstanceEntity> ProductInstanceEntities { get; set; }

        public virtual DbSet<WorkplanEntity> WorkplanEntities { get; set; }

        public virtual DbSet<WorkplanReference> WorkplanReferenceEntities { get; set; }

        public virtual DbSet<StepEntity> StepEntities { get; set; }

        public virtual DbSet<ConnectorEntity> ConnectorEntities { get; set; }

        public virtual DbSet<ConnectorReference> ConnectorReferenceEntities { get; set; }

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

            // Product Instances
            modelBuilder.Entity<ProductInstanceEntity>()
                .HasRequired(p => p.Product)
                .WithMany()
                .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<ProductInstanceEntity>()
                .HasOptional(a => a.Parent)
                .WithMany(a => a.Parts)
                .HasForeignKey(a => a.ParentId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ProductInstanceEntity>()
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

            // RecipeEntity
            modelBuilder.Entity<ProductRecipeEntity>()
                .HasOptional(r => r.Workplan)
                .WithMany(w => w.Recipes)
                .HasForeignKey(s => s.WorkplanId);

            // ProductEntity
            modelBuilder.Entity<ProductTypeEntity>()
                .HasMany(p => p.Parts)
                .WithRequired(p => p.Parent)
                .HasForeignKey(p => p.ParentId);

            modelBuilder.Entity<ProductTypeEntity>()
                .HasMany(p => p.Parents)
                .WithRequired(p => p.Child)
                .HasForeignKey(p => p.ChildId);

            modelBuilder.Entity<ProductTypeEntity>()
                .HasMany(p => p.Recipes)
                .WithRequired(p => p.Product)
                .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<ProductTypeEntity>()
                .HasMany(p => p.OldVersions)
                .WithOptional(p => p.Product)
                .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<ProductTypeEntity>()
                .HasRequired(p => p.CurrentVersion)
                .WithMany()
                .HasForeignKey(t => t.CurrentVersionId);

            // Indexes
            modelBuilder.Entity<ProductTypeEntity>()
                .Property(e => e.Identifier)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new[]
                    {
                        new IndexAttribute("Identifier_Revision_Index", 1),
                        new IndexAttribute("Identifier")
                    }));

            modelBuilder.Entity<ProductTypeEntity>()
                .Property(e => e.Revision)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("Identifier_Revision_Index", 2)));
        }
    }
}
