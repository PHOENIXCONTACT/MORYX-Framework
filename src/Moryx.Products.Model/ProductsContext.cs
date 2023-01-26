// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moryx.Model;
using Moryx.Model.Attributes;
using Moryx.Model.PostgreSQL;

namespace Moryx.Products.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    [ModelConfigurator(typeof(NpgsqlModelConfigurator))]
    public class ProductsContext : MoryxDbContext
    {
        /// <inheritdoc />
        public ProductsContext()
        {
        }

        /// <inheritdoc />
        public ProductsContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<ProductTypeEntity> ProductTypes { get; set; }

        public virtual DbSet<PartLinkEntity> PartLinks { get; set; }

        public virtual DbSet<ProductRecipeEntity> ProductRecipes { get; set; }

        public virtual DbSet<ProductTypePropertiesEntity> ProductTypeProperties { get; set; }

        public virtual DbSet<ProductFileEntity> ProductFiles { get; set; }

        public virtual DbSet<ProductInstanceEntity> ProductInstances { get; set; }

        public virtual DbSet<WorkplanEntity> Workplans { get; set; }

        public virtual DbSet<WorkplanReferenceEntity> WorkplanReferences { get; set; }

        public virtual DbSet<WorkplanStepEntity> WorkplanSteps { get; set; }

        public virtual DbSet<WorkplanConnectorEntity> WorkplanConnectorEntities { get; set; }

        public virtual DbSet<WorkplanConnectorReferenceEntity> WorkplanConnectorReferences { get; set; }

        public virtual DbSet<WorkplanOutputDescriptionEntity> WorkplanOutputDescriptions { get; set; }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                var connectionString = configuration.GetConnectionString("Moryx.Products.Model");
                optionsBuilder.UseNpgsql(connectionString);
            }

            optionsBuilder.UseLazyLoadingProxies();
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Workplane reference
            modelBuilder.Entity<WorkplanReferenceEntity>()
                .HasOne(w => w.Target)
                .WithMany(w => w.TargetReferences).IsRequired()
                .HasForeignKey(s => s.TargetId);

            modelBuilder.Entity<WorkplanReferenceEntity>()
                .HasOne(w => w.Source)
                .WithMany(w => w.SourceReferences).IsRequired()
                .HasForeignKey(s => s.SourceId);

            // Product Instances
            modelBuilder.Entity<ProductInstanceEntity>()
                .HasOne(p => p.Product)
                .WithMany().IsRequired()
                .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<ProductInstanceEntity>()
                .HasOne(a => a.Parent)
                .WithMany(a => a.Parts)
                .HasForeignKey(a => a.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductInstanceEntity>()
                .HasOne(a => a.PartLinkEntity)
                .WithMany()
                .HasForeignKey(a => a.PartLinkEntityId);

            // Connector
            modelBuilder.Entity<WorkplanConnectorEntity>()
                .HasOne(c => c.Workplan)
                .WithMany(w => w.Connectors).IsRequired()
                .HasForeignKey(c => c.WorkplanId);

            modelBuilder.Entity<WorkplanConnectorEntity>()
                .HasMany(c => c.Usages)
                .WithOne(c => c.Connector)
                .HasForeignKey(c => c.ConnectorId);

            // Step entity
            modelBuilder.Entity<WorkplanStepEntity>()
                .HasMany(s => s.Connectors);

            modelBuilder.Entity<WorkplanStepEntity>()
                .HasMany(s => s.OutputDescriptions)
                .WithOne(o => o.WorkplanStep).IsRequired()
                .HasForeignKey(o => o.WorkplanStepId);

            modelBuilder.Entity<WorkplanStepEntity>()
                .HasOne(s => s.Workplan)
                .WithMany(w => w.Steps).IsRequired()
                .HasForeignKey(s => s.WorkplanId);

            modelBuilder.Entity<WorkplanStepEntity>()
                .HasOne(s => s.SubWorkplan)
                .WithMany(w => w.Parents)
                .HasForeignKey(s => s.SubWorkplanId);

            // RecipeEntity
            modelBuilder.Entity<ProductRecipeEntity>()
                .HasOne(r => r.Workplan)
                .WithMany(w => w.Recipes)
                .HasForeignKey(s => s.WorkplanId);

            // ProductEntity
            modelBuilder.Entity<ProductTypeEntity>()
                .HasMany(p => p.Parts)
                .WithOne(p => p.Parent).IsRequired()
                .HasForeignKey(p => p.ParentId);

            modelBuilder.Entity<ProductTypeEntity>()
                .HasMany(p => p.Parents)
                .WithOne(p => p.Child).IsRequired()
                .HasForeignKey(p => p.ChildId);

            modelBuilder.Entity<ProductTypeEntity>()
                .HasMany(p => p.Recipes)
                .WithOne(p => p.Product).IsRequired()
                .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<ProductTypeEntity>()
                .HasMany(p => p.OldVersions)
                .WithOne(p => p.Product)
                .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<ProductTypeEntity>()
                .HasOne(p => p.CurrentVersion)
                .WithMany().IsRequired()
                .HasForeignKey(t => t.CurrentVersionId);

            // Indexes
            modelBuilder.Entity<ProductTypeEntity>()
                .HasIndex(p => new { p.Identifier, p.Revision});

            modelBuilder.Entity<ProductTypeEntity>()
                .HasIndex(p => new { p.Identifier, p.Revision });
        }
    }
}
