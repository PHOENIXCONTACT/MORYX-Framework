﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Moryx.Products.Model;

#nullable disable

namespace Moryx.Products.Model.Migrations.Sqlite
{
    [DbContext(typeof(SqliteProductsContext))]
    [Migration("20230214165158_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("public")
                .HasAnnotation("ProductVersion", "6.0.0");

            modelBuilder.Entity("Moryx.Products.Model.PartLinkEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("ChildId")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Float1")
                        .HasColumnType("REAL");

                    b.Property<double>("Float2")
                        .HasColumnType("REAL");

                    b.Property<double>("Float3")
                        .HasColumnType("REAL");

                    b.Property<double>("Float4")
                        .HasColumnType("REAL");

                    b.Property<double>("Float5")
                        .HasColumnType("REAL");

                    b.Property<double>("Float6")
                        .HasColumnType("REAL");

                    b.Property<double>("Float7")
                        .HasColumnType("REAL");

                    b.Property<double>("Float8")
                        .HasColumnType("REAL");

                    b.Property<long>("Integer1")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer2")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer3")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer4")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer5")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer6")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer7")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer8")
                        .HasColumnType("INTEGER");

                    b.Property<long>("ParentId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PropertyName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text1")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text2")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text3")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text4")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text5")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text6")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text7")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text8")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ChildId");

                    b.HasIndex("ParentId");

                    b.ToTable("PartLinks", "public");
                });

            modelBuilder.Entity("Moryx.Products.Model.ProductFileEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FileHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("FilePath")
                        .HasColumnType("TEXT");

                    b.Property<string>("MimeType")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<long?>("ProductId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Version")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductFiles", "public");
                });

            modelBuilder.Entity("Moryx.Products.Model.ProductInstanceEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("Float1")
                        .HasColumnType("REAL");

                    b.Property<double>("Float2")
                        .HasColumnType("REAL");

                    b.Property<double>("Float3")
                        .HasColumnType("REAL");

                    b.Property<double>("Float4")
                        .HasColumnType("REAL");

                    b.Property<double>("Float5")
                        .HasColumnType("REAL");

                    b.Property<double>("Float6")
                        .HasColumnType("REAL");

                    b.Property<double>("Float7")
                        .HasColumnType("REAL");

                    b.Property<double>("Float8")
                        .HasColumnType("REAL");

                    b.Property<long>("Integer1")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer2")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer3")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer4")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer5")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer6")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer7")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer8")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("ParentId")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("PartLinkEntityId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("ProductId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("State")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Text1")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text2")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text3")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text4")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text5")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text6")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text7")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text8")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.HasIndex("PartLinkEntityId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductInstances", "public");
                });

            modelBuilder.Entity("Moryx.Products.Model.ProductRecipeEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Classification")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Deleted")
                        .HasColumnType("TEXT");

                    b.Property<double>("Float1")
                        .HasColumnType("REAL");

                    b.Property<double>("Float2")
                        .HasColumnType("REAL");

                    b.Property<double>("Float3")
                        .HasColumnType("REAL");

                    b.Property<double>("Float4")
                        .HasColumnType("REAL");

                    b.Property<double>("Float5")
                        .HasColumnType("REAL");

                    b.Property<double>("Float6")
                        .HasColumnType("REAL");

                    b.Property<double>("Float7")
                        .HasColumnType("REAL");

                    b.Property<double>("Float8")
                        .HasColumnType("REAL");

                    b.Property<long>("Integer1")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer2")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer3")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer4")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer5")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer6")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer7")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer8")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<long>("ProductId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Revision")
                        .HasColumnType("INTEGER");

                    b.Property<int>("State")
                        .HasColumnType("INTEGER");

                    b.Property<long>("TemplateId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Text1")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text2")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text3")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text4")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text5")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text6")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text7")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text8")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("TEXT");

                    b.Property<long?>("WorkplanId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("WorkplanId");

                    b.ToTable("ProductRecipes", "public");
                });

            modelBuilder.Entity("Moryx.Products.Model.ProductTypeEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<long>("CurrentVersionId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("Deleted")
                        .HasColumnType("TEXT");

                    b.Property<string>("Identifier")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<short>("Revision")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TypeName")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CurrentVersionId");

                    b.HasIndex("Identifier", "Revision");

                    b.ToTable("ProductTypes", "public");
                });

            modelBuilder.Entity("Moryx.Products.Model.ProductTypePropertiesEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Deleted")
                        .HasColumnType("TEXT");

                    b.Property<double>("Float1")
                        .HasColumnType("REAL");

                    b.Property<double>("Float2")
                        .HasColumnType("REAL");

                    b.Property<double>("Float3")
                        .HasColumnType("REAL");

                    b.Property<double>("Float4")
                        .HasColumnType("REAL");

                    b.Property<double>("Float5")
                        .HasColumnType("REAL");

                    b.Property<double>("Float6")
                        .HasColumnType("REAL");

                    b.Property<double>("Float7")
                        .HasColumnType("REAL");

                    b.Property<double>("Float8")
                        .HasColumnType("REAL");

                    b.Property<long>("Integer1")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer2")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer3")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer4")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer5")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer6")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer7")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Integer8")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("ProductId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("State")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Text1")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text2")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text3")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text4")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text5")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text6")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text7")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text8")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductTypeProperties", "public");
                });

            modelBuilder.Entity("Moryx.Products.Model.WorkplanConnectorEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Classification")
                        .HasColumnType("INTEGER");

                    b.Property<long>("ConnectorId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("PositionX")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PositionY")
                        .HasColumnType("INTEGER");

                    b.Property<long>("WorkplanId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("WorkplanId");

                    b.ToTable("WorkplanConnectorEntities", "public");
                });

            modelBuilder.Entity("Moryx.Products.Model.WorkplanConnectorReferenceEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long?>("ConnectorId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Index")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Role")
                        .HasColumnType("INTEGER");

                    b.Property<long>("WorkplanStepId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ConnectorId");

                    b.HasIndex("WorkplanStepId");

                    b.ToTable("WorkplanConnectorReferences", "public");
                });

            modelBuilder.Entity("Moryx.Products.Model.WorkplanEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Deleted")
                        .HasColumnType("TEXT");

                    b.Property<int>("MaxElementId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("State")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("TEXT");

                    b.Property<int>("Version")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Workplans", "public");
                });

            modelBuilder.Entity("Moryx.Products.Model.WorkplanOutputDescriptionEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Index")
                        .HasColumnType("INTEGER");

                    b.Property<long>("MappingValue")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("OutputType")
                        .HasColumnType("INTEGER");

                    b.Property<long>("WorkplanStepId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("WorkplanStepId");

                    b.ToTable("WorkplanOutputDescriptions", "public");
                });

            modelBuilder.Entity("Moryx.Products.Model.WorkplanReferenceEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ReferenceType")
                        .HasColumnType("INTEGER");

                    b.Property<long>("SourceId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("TargetId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SourceId");

                    b.HasIndex("TargetId");

                    b.ToTable("WorkplanReferences", "public");
                });

            modelBuilder.Entity("Moryx.Products.Model.WorkplanStepEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Assembly")
                        .HasColumnType("TEXT");

                    b.Property<string>("Classname")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("NameSpace")
                        .HasColumnType("TEXT");

                    b.Property<string>("Parameters")
                        .HasColumnType("TEXT");

                    b.Property<int>("PositionX")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PositionY")
                        .HasColumnType("INTEGER");

                    b.Property<long>("StepId")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("SubWorkplanId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("WorkplanId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SubWorkplanId");

                    b.HasIndex("WorkplanId");

                    b.ToTable("WorkplanSteps", "public");
                });

            modelBuilder.Entity("Moryx.Products.Model.PartLinkEntity", b =>
                {
                    b.HasOne("Moryx.Products.Model.ProductTypeEntity", "Child")
                        .WithMany("Parents")
                        .HasForeignKey("ChildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Moryx.Products.Model.ProductTypeEntity", "Parent")
                        .WithMany("Parts")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Child");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("Moryx.Products.Model.ProductFileEntity", b =>
                {
                    b.HasOne("Moryx.Products.Model.ProductTypeEntity", "Product")
                        .WithMany("Files")
                        .HasForeignKey("ProductId");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Moryx.Products.Model.ProductInstanceEntity", b =>
                {
                    b.HasOne("Moryx.Products.Model.ProductInstanceEntity", "Parent")
                        .WithMany("Parts")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Moryx.Products.Model.PartLinkEntity", "PartLinkEntity")
                        .WithMany()
                        .HasForeignKey("PartLinkEntityId");

                    b.HasOne("Moryx.Products.Model.ProductTypeEntity", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Parent");

                    b.Navigation("PartLinkEntity");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Moryx.Products.Model.ProductRecipeEntity", b =>
                {
                    b.HasOne("Moryx.Products.Model.ProductTypeEntity", "Product")
                        .WithMany("Recipes")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Moryx.Products.Model.WorkplanEntity", "Workplan")
                        .WithMany("Recipes")
                        .HasForeignKey("WorkplanId");

                    b.Navigation("Product");

                    b.Navigation("Workplan");
                });

            modelBuilder.Entity("Moryx.Products.Model.ProductTypeEntity", b =>
                {
                    b.HasOne("Moryx.Products.Model.ProductTypePropertiesEntity", "CurrentVersion")
                        .WithMany()
                        .HasForeignKey("CurrentVersionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CurrentVersion");
                });

            modelBuilder.Entity("Moryx.Products.Model.ProductTypePropertiesEntity", b =>
                {
                    b.HasOne("Moryx.Products.Model.ProductTypeEntity", "Product")
                        .WithMany("OldVersions")
                        .HasForeignKey("ProductId");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Moryx.Products.Model.WorkplanConnectorEntity", b =>
                {
                    b.HasOne("Moryx.Products.Model.WorkplanEntity", "Workplan")
                        .WithMany("Connectors")
                        .HasForeignKey("WorkplanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Workplan");
                });

            modelBuilder.Entity("Moryx.Products.Model.WorkplanConnectorReferenceEntity", b =>
                {
                    b.HasOne("Moryx.Products.Model.WorkplanConnectorEntity", "Connector")
                        .WithMany("Usages")
                        .HasForeignKey("ConnectorId");

                    b.HasOne("Moryx.Products.Model.WorkplanStepEntity", "WorkplanStep")
                        .WithMany("Connectors")
                        .HasForeignKey("WorkplanStepId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Connector");

                    b.Navigation("WorkplanStep");
                });

            modelBuilder.Entity("Moryx.Products.Model.WorkplanOutputDescriptionEntity", b =>
                {
                    b.HasOne("Moryx.Products.Model.WorkplanStepEntity", "WorkplanStep")
                        .WithMany("OutputDescriptions")
                        .HasForeignKey("WorkplanStepId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WorkplanStep");
                });

            modelBuilder.Entity("Moryx.Products.Model.WorkplanReferenceEntity", b =>
                {
                    b.HasOne("Moryx.Products.Model.WorkplanEntity", "Source")
                        .WithMany("SourceReferences")
                        .HasForeignKey("SourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Moryx.Products.Model.WorkplanEntity", "Target")
                        .WithMany("TargetReferences")
                        .HasForeignKey("TargetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Source");

                    b.Navigation("Target");
                });

            modelBuilder.Entity("Moryx.Products.Model.WorkplanStepEntity", b =>
                {
                    b.HasOne("Moryx.Products.Model.WorkplanEntity", "SubWorkplan")
                        .WithMany("Parents")
                        .HasForeignKey("SubWorkplanId");

                    b.HasOne("Moryx.Products.Model.WorkplanEntity", "Workplan")
                        .WithMany("Steps")
                        .HasForeignKey("WorkplanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SubWorkplan");

                    b.Navigation("Workplan");
                });

            modelBuilder.Entity("Moryx.Products.Model.ProductInstanceEntity", b =>
                {
                    b.Navigation("Parts");
                });

            modelBuilder.Entity("Moryx.Products.Model.ProductTypeEntity", b =>
                {
                    b.Navigation("Files");

                    b.Navigation("OldVersions");

                    b.Navigation("Parents");

                    b.Navigation("Parts");

                    b.Navigation("Recipes");
                });

            modelBuilder.Entity("Moryx.Products.Model.WorkplanConnectorEntity", b =>
                {
                    b.Navigation("Usages");
                });

            modelBuilder.Entity("Moryx.Products.Model.WorkplanEntity", b =>
                {
                    b.Navigation("Connectors");

                    b.Navigation("Parents");

                    b.Navigation("Recipes");

                    b.Navigation("SourceReferences");

                    b.Navigation("Steps");

                    b.Navigation("TargetReferences");
                });

            modelBuilder.Entity("Moryx.Products.Model.WorkplanStepEntity", b =>
                {
                    b.Navigation("Connectors");

                    b.Navigation("OutputDescriptions");
                });
#pragma warning restore 612, 618
        }
    }
}
