﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Moryx.Products.Model;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Moryx.Products.Model.Migrations.Npgsql
{
    [DbContext(typeof(NpgsqlProductsContext))]
    partial class ProductsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("public")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Moryx.Products.Model.PartLinkEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long>("ChildId")
                        .HasColumnType("bigint");

                    b.Property<double>("Float1")
                        .HasColumnType("double precision");

                    b.Property<double>("Float2")
                        .HasColumnType("double precision");

                    b.Property<double>("Float3")
                        .HasColumnType("double precision");

                    b.Property<double>("Float4")
                        .HasColumnType("double precision");

                    b.Property<double>("Float5")
                        .HasColumnType("double precision");

                    b.Property<double>("Float6")
                        .HasColumnType("double precision");

                    b.Property<double>("Float7")
                        .HasColumnType("double precision");

                    b.Property<double>("Float8")
                        .HasColumnType("double precision");

                    b.Property<long>("Integer1")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer2")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer3")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer4")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer5")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer6")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer7")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer8")
                        .HasColumnType("bigint");

                    b.Property<long>("ParentId")
                        .HasColumnType("bigint");

                    b.Property<string>("PropertyName")
                        .HasColumnType("text");

                    b.Property<string>("Text1")
                        .HasColumnType("text");

                    b.Property<string>("Text2")
                        .HasColumnType("text");

                    b.Property<string>("Text3")
                        .HasColumnType("text");

                    b.Property<string>("Text4")
                        .HasColumnType("text");

                    b.Property<string>("Text5")
                        .HasColumnType("text");

                    b.Property<string>("Text6")
                        .HasColumnType("text");

                    b.Property<string>("Text7")
                        .HasColumnType("text");

                    b.Property<string>("Text8")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ChildId");

                    b.HasIndex("ParentId");

                    b.ToTable("PartLinks");
                });

            modelBuilder.Entity("Moryx.Products.Model.ProductFileEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("FileHash")
                        .HasColumnType("text");

                    b.Property<string>("FilePath")
                        .HasColumnType("text");

                    b.Property<string>("MimeType")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<long?>("ProductId")
                        .HasColumnType("bigint");

                    b.Property<int>("Version")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductFiles");
                });

            modelBuilder.Entity("Moryx.Products.Model.ProductInstanceEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<double>("Float1")
                        .HasColumnType("double precision");

                    b.Property<double>("Float2")
                        .HasColumnType("double precision");

                    b.Property<double>("Float3")
                        .HasColumnType("double precision");

                    b.Property<double>("Float4")
                        .HasColumnType("double precision");

                    b.Property<double>("Float5")
                        .HasColumnType("double precision");

                    b.Property<double>("Float6")
                        .HasColumnType("double precision");

                    b.Property<double>("Float7")
                        .HasColumnType("double precision");

                    b.Property<double>("Float8")
                        .HasColumnType("double precision");

                    b.Property<long>("Integer1")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer2")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer3")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer4")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer5")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer6")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer7")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer8")
                        .HasColumnType("bigint");

                    b.Property<long?>("ParentId")
                        .HasColumnType("bigint");

                    b.Property<long?>("PartLinkEntityId")
                        .HasColumnType("bigint");

                    b.Property<long>("ProductId")
                        .HasColumnType("bigint");

                    b.Property<long>("State")
                        .HasColumnType("bigint");

                    b.Property<string>("Text1")
                        .HasColumnType("text");

                    b.Property<string>("Text2")
                        .HasColumnType("text");

                    b.Property<string>("Text3")
                        .HasColumnType("text");

                    b.Property<string>("Text4")
                        .HasColumnType("text");

                    b.Property<string>("Text5")
                        .HasColumnType("text");

                    b.Property<string>("Text6")
                        .HasColumnType("text");

                    b.Property<string>("Text7")
                        .HasColumnType("text");

                    b.Property<string>("Text8")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.HasIndex("PartLinkEntityId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductInstances");
                });

            modelBuilder.Entity("Moryx.Products.Model.ProductRecipeEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("Classification")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("Deleted")
                        .HasColumnType("timestamp without time zone");

                    b.Property<double>("Float1")
                        .HasColumnType("double precision");

                    b.Property<double>("Float2")
                        .HasColumnType("double precision");

                    b.Property<double>("Float3")
                        .HasColumnType("double precision");

                    b.Property<double>("Float4")
                        .HasColumnType("double precision");

                    b.Property<double>("Float5")
                        .HasColumnType("double precision");

                    b.Property<double>("Float6")
                        .HasColumnType("double precision");

                    b.Property<double>("Float7")
                        .HasColumnType("double precision");

                    b.Property<double>("Float8")
                        .HasColumnType("double precision");

                    b.Property<long>("Integer1")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer2")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer3")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer4")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer5")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer6")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer7")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer8")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<long>("ProductId")
                        .HasColumnType("bigint");

                    b.Property<int>("Revision")
                        .HasColumnType("integer");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.Property<long>("TemplateId")
                        .HasColumnType("bigint");

                    b.Property<string>("Text1")
                        .HasColumnType("text");

                    b.Property<string>("Text2")
                        .HasColumnType("text");

                    b.Property<string>("Text3")
                        .HasColumnType("text");

                    b.Property<string>("Text4")
                        .HasColumnType("text");

                    b.Property<string>("Text5")
                        .HasColumnType("text");

                    b.Property<string>("Text6")
                        .HasColumnType("text");

                    b.Property<string>("Text7")
                        .HasColumnType("text");

                    b.Property<string>("Text8")
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .HasColumnType("text");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long?>("WorkplanId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("WorkplanId");

                    b.ToTable("ProductRecipes");
                });

            modelBuilder.Entity("Moryx.Products.Model.ProductTypeEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("CurrentVersionId")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("Deleted")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Identifier")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<short>("Revision")
                        .HasColumnType("smallint");

                    b.Property<string>("TypeName")
                        .HasColumnType("text");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("CurrentVersionId");

                    b.HasIndex("Identifier", "Revision");

                    b.ToTable("ProductTypes");
                });

            modelBuilder.Entity("Moryx.Products.Model.ProductTypePropertiesEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("Deleted")
                        .HasColumnType("timestamp without time zone");

                    b.Property<double>("Float1")
                        .HasColumnType("double precision");

                    b.Property<double>("Float2")
                        .HasColumnType("double precision");

                    b.Property<double>("Float3")
                        .HasColumnType("double precision");

                    b.Property<double>("Float4")
                        .HasColumnType("double precision");

                    b.Property<double>("Float5")
                        .HasColumnType("double precision");

                    b.Property<double>("Float6")
                        .HasColumnType("double precision");

                    b.Property<double>("Float7")
                        .HasColumnType("double precision");

                    b.Property<double>("Float8")
                        .HasColumnType("double precision");

                    b.Property<long>("Integer1")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer2")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer3")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer4")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer5")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer6")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer7")
                        .HasColumnType("bigint");

                    b.Property<long>("Integer8")
                        .HasColumnType("bigint");

                    b.Property<long?>("ProductId")
                        .HasColumnType("bigint");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.Property<string>("Text1")
                        .HasColumnType("text");

                    b.Property<string>("Text2")
                        .HasColumnType("text");

                    b.Property<string>("Text3")
                        .HasColumnType("text");

                    b.Property<string>("Text4")
                        .HasColumnType("text");

                    b.Property<string>("Text5")
                        .HasColumnType("text");

                    b.Property<string>("Text6")
                        .HasColumnType("text");

                    b.Property<string>("Text7")
                        .HasColumnType("text");

                    b.Property<string>("Text8")
                        .HasColumnType("text");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductTypeProperties");
                });

            modelBuilder.Entity("Moryx.Products.Model.WorkplanConnectorEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("Classification")
                        .HasColumnType("integer");

                    b.Property<long>("ConnectorId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<long>("WorkplanId")
                        .HasColumnType("bigint");

                    b.Property<int>("PositionX")
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.Property<int>("PositionY")
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.HasKey("Id");

                    b.HasIndex("WorkplanId");

                    b.ToTable("WorkplanConnectorEntities");
                });

            modelBuilder.Entity("Moryx.Products.Model.WorkplanConnectorReferenceEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long?>("ConnectorId")
                        .HasColumnType("bigint");

                    b.Property<int>("Index")
                        .HasColumnType("integer");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<long>("WorkplanStepId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ConnectorId");

                    b.HasIndex("WorkplanStepId");

                    b.ToTable("WorkplanConnectorReferences");
                });

            modelBuilder.Entity("Moryx.Products.Model.WorkplanEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("Deleted")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("MaxElementId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("Version")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Workplans");
                });

            modelBuilder.Entity("Moryx.Products.Model.WorkplanOutputDescriptionEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("Index")
                        .HasColumnType("integer");

                    b.Property<long>("MappingValue")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("OutputType")
                        .HasColumnType("integer");

                    b.Property<long>("WorkplanStepId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("WorkplanStepId");

                    b.ToTable("WorkplanOutputDescriptions");
                });

            modelBuilder.Entity("Moryx.Products.Model.WorkplanReferenceEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ReferenceType")
                        .HasColumnType("integer");

                    b.Property<long>("SourceId")
                        .HasColumnType("bigint");

                    b.Property<long>("TargetId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("SourceId");

                    b.HasIndex("TargetId");

                    b.ToTable("WorkplanReferences");
                });

            modelBuilder.Entity("Moryx.Products.Model.WorkplanStepEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Assembly")
                        .HasColumnType("text");

                    b.Property<string>("Classname")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("NameSpace")
                        .HasColumnType("text");

                    b.Property<string>("Parameters")
                        .HasColumnType("text");

                    b.Property<long>("StepId")
                        .HasColumnType("bigint");

                    b.Property<long?>("SubWorkplanId")
                        .HasColumnType("bigint");

                    b.Property<long>("WorkplanId")
                        .HasColumnType("bigint");

                    b.Property<int>("PositionX")
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.Property<int>("PositionY")
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.HasKey("Id");

                    b.HasIndex("SubWorkplanId");

                    b.HasIndex("WorkplanId");

                    b.ToTable("WorkplanSteps");
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
                });

            modelBuilder.Entity("Moryx.Products.Model.ProductFileEntity", b =>
                {
                    b.HasOne("Moryx.Products.Model.ProductTypeEntity", "Product")
                        .WithMany("Files")
                        .HasForeignKey("ProductId");
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
                });

            modelBuilder.Entity("Moryx.Products.Model.ProductTypeEntity", b =>
                {
                    b.HasOne("Moryx.Products.Model.ProductTypePropertiesEntity", "CurrentVersion")
                        .WithMany()
                        .HasForeignKey("CurrentVersionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Moryx.Products.Model.ProductTypePropertiesEntity", b =>
                {
                    b.HasOne("Moryx.Products.Model.ProductTypeEntity", "Product")
                        .WithMany("OldVersions")
                        .HasForeignKey("ProductId");
                });

            modelBuilder.Entity("Moryx.Products.Model.WorkplanConnectorEntity", b =>
                {
                    b.HasOne("Moryx.Products.Model.WorkplanEntity", "Workplan")
                        .WithMany("Connectors")
                        .HasForeignKey("WorkplanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
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
                });

            modelBuilder.Entity("Moryx.Products.Model.WorkplanOutputDescriptionEntity", b =>
                {
                    b.HasOne("Moryx.Products.Model.WorkplanStepEntity", "WorkplanStep")
                        .WithMany("OutputDescriptions")
                        .HasForeignKey("WorkplanStepId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
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
                });
#pragma warning restore 612, 618
        }
    }
}
