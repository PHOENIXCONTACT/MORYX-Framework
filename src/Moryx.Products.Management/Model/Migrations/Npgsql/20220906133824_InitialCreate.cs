// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Moryx.Products.Management.Model.Migrations.Npgsql;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "public");

        migrationBuilder.CreateTable(
            name: "Workplans",
            schema: "public",
            columns: table => new
            {
                Id = table.Column<long>(nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Created = table.Column<DateTime>(nullable: false),
                Updated = table.Column<DateTime>(nullable: false),
                Deleted = table.Column<DateTime>(nullable: true),
                Name = table.Column<string>(nullable: true),
                Version = table.Column<int>(nullable: false),
                State = table.Column<int>(nullable: false),
                MaxElementId = table.Column<int>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Workplans", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "WorkplanConnectorEntities",
            schema: "public",
            columns: table => new
            {
                Id = table.Column<long>(nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ConnectorId = table.Column<long>(nullable: false),
                Name = table.Column<string>(nullable: true),
                Classification = table.Column<int>(nullable: false),
                WorkplanId = table.Column<long>(nullable: false),
                PositionX = table.Column<int>(nullable: false, defaultValue: 0),
                PositionY = table.Column<int>(nullable: false, defaultValue: 0)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WorkplanConnectorEntities", x => x.Id);
                table.ForeignKey(
                    name: "FK_WorkplanConnectorEntities_Workplans_WorkplanId",
                    column: x => x.WorkplanId,
                    principalSchema: "public",
                    principalTable: "Workplans",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "WorkplanReferences",
            schema: "public",
            columns: table => new
            {
                Id = table.Column<long>(nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ReferenceType = table.Column<int>(nullable: false),
                SourceId = table.Column<long>(nullable: false),
                TargetId = table.Column<long>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WorkplanReferences", x => x.Id);
                table.ForeignKey(
                    name: "FK_WorkplanReferences_Workplans_SourceId",
                    column: x => x.SourceId,
                    principalSchema: "public",
                    principalTable: "Workplans",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_WorkplanReferences_Workplans_TargetId",
                    column: x => x.TargetId,
                    principalSchema: "public",
                    principalTable: "Workplans",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "WorkplanSteps",
            schema: "public",
            columns: table => new
            {
                Id = table.Column<long>(nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                StepId = table.Column<long>(nullable: false),
                Name = table.Column<string>(nullable: true),
                Assembly = table.Column<string>(nullable: true),
                NameSpace = table.Column<string>(nullable: true),
                Classname = table.Column<string>(nullable: true),
                Parameters = table.Column<string>(nullable: true),
                WorkplanId = table.Column<long>(nullable: false),
                SubWorkplanId = table.Column<long>(nullable: true),
                PositionX = table.Column<int>(nullable: false, defaultValue: 0),
                PositionY = table.Column<int>(nullable: false, defaultValue: 0)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WorkplanSteps", x => x.Id);
                table.ForeignKey(
                    name: "FK_WorkplanSteps_Workplans_SubWorkplanId",
                    column: x => x.SubWorkplanId,
                    principalSchema: "public",
                    principalTable: "Workplans",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_WorkplanSteps_Workplans_WorkplanId",
                    column: x => x.WorkplanId,
                    principalSchema: "public",
                    principalTable: "Workplans",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "WorkplanConnectorReferences",
            schema: "public",
            columns: table => new
            {
                Id = table.Column<long>(nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Index = table.Column<int>(nullable: false),
                Role = table.Column<int>(nullable: false),
                ConnectorId = table.Column<long>(nullable: true),
                WorkplanStepId = table.Column<long>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WorkplanConnectorReferences", x => x.Id);
                table.ForeignKey(
                    name: "FK_WorkplanConnectorReferences_WorkplanConnectorEntities_Conne~",
                    column: x => x.ConnectorId,
                    principalSchema: "public",
                    principalTable: "WorkplanConnectorEntities",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_WorkplanConnectorReferences_WorkplanSteps_WorkplanStepId",
                    column: x => x.WorkplanStepId,
                    principalSchema: "public",
                    principalTable: "WorkplanSteps",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "WorkplanOutputDescriptions",
            schema: "public",
            columns: table => new
            {
                Id = table.Column<long>(nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Index = table.Column<int>(nullable: false),
                OutputType = table.Column<int>(nullable: false),
                Name = table.Column<string>(nullable: true),
                MappingValue = table.Column<long>(nullable: false),
                WorkplanStepId = table.Column<long>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WorkplanOutputDescriptions", x => x.Id);
                table.ForeignKey(
                    name: "FK_WorkplanOutputDescriptions_WorkplanSteps_WorkplanStepId",
                    column: x => x.WorkplanStepId,
                    principalSchema: "public",
                    principalTable: "WorkplanSteps",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ProductInstances",
            schema: "public",
            columns: table => new
            {
                Id = table.Column<long>(nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                State = table.Column<long>(nullable: false),
                ProductId = table.Column<long>(nullable: false),
                ParentId = table.Column<long>(nullable: true),
                PartLinkEntityId = table.Column<long>(nullable: true),
                Integer1 = table.Column<long>(nullable: false),
                Integer2 = table.Column<long>(nullable: false),
                Integer3 = table.Column<long>(nullable: false),
                Integer4 = table.Column<long>(nullable: false),
                Integer5 = table.Column<long>(nullable: false),
                Integer6 = table.Column<long>(nullable: false),
                Integer7 = table.Column<long>(nullable: false),
                Integer8 = table.Column<long>(nullable: false),
                Float1 = table.Column<double>(nullable: false),
                Float2 = table.Column<double>(nullable: false),
                Float3 = table.Column<double>(nullable: false),
                Float4 = table.Column<double>(nullable: false),
                Float5 = table.Column<double>(nullable: false),
                Float6 = table.Column<double>(nullable: false),
                Float7 = table.Column<double>(nullable: false),
                Float8 = table.Column<double>(nullable: false),
                Text1 = table.Column<string>(nullable: true),
                Text2 = table.Column<string>(nullable: true),
                Text3 = table.Column<string>(nullable: true),
                Text4 = table.Column<string>(nullable: true),
                Text5 = table.Column<string>(nullable: true),
                Text6 = table.Column<string>(nullable: true),
                Text7 = table.Column<string>(nullable: true),
                Text8 = table.Column<string>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProductInstances", x => x.Id);
                table.ForeignKey(
                    name: "FK_ProductInstances_ProductInstances_ParentId",
                    column: x => x.ParentId,
                    principalSchema: "public",
                    principalTable: "ProductInstances",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ProductTypes",
            schema: "public",
            columns: table => new
            {
                Id = table.Column<long>(nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Created = table.Column<DateTime>(nullable: false),
                Updated = table.Column<DateTime>(nullable: false),
                Deleted = table.Column<DateTime>(nullable: true),
                TypeName = table.Column<string>(nullable: true),
                Identifier = table.Column<string>(nullable: true),
                Revision = table.Column<short>(nullable: false),
                Name = table.Column<string>(nullable: true),
                CurrentVersionId = table.Column<long>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProductTypes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "PartLinks",
            schema: "public",
            columns: table => new
            {
                Id = table.Column<long>(nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ParentId = table.Column<long>(nullable: false),
                ChildId = table.Column<long>(nullable: false),
                PropertyName = table.Column<string>(nullable: true),
                Integer1 = table.Column<long>(nullable: false),
                Integer2 = table.Column<long>(nullable: false),
                Integer3 = table.Column<long>(nullable: false),
                Integer4 = table.Column<long>(nullable: false),
                Integer5 = table.Column<long>(nullable: false),
                Integer6 = table.Column<long>(nullable: false),
                Integer7 = table.Column<long>(nullable: false),
                Integer8 = table.Column<long>(nullable: false),
                Float1 = table.Column<double>(nullable: false),
                Float2 = table.Column<double>(nullable: false),
                Float3 = table.Column<double>(nullable: false),
                Float4 = table.Column<double>(nullable: false),
                Float5 = table.Column<double>(nullable: false),
                Float6 = table.Column<double>(nullable: false),
                Float7 = table.Column<double>(nullable: false),
                Float8 = table.Column<double>(nullable: false),
                Text1 = table.Column<string>(nullable: true),
                Text2 = table.Column<string>(nullable: true),
                Text3 = table.Column<string>(nullable: true),
                Text4 = table.Column<string>(nullable: true),
                Text5 = table.Column<string>(nullable: true),
                Text6 = table.Column<string>(nullable: true),
                Text7 = table.Column<string>(nullable: true),
                Text8 = table.Column<string>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PartLinks", x => x.Id);
                table.ForeignKey(
                    name: "FK_PartLinks_ProductTypes_ChildId",
                    column: x => x.ChildId,
                    principalSchema: "public",
                    principalTable: "ProductTypes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_PartLinks_ProductTypes_ParentId",
                    column: x => x.ParentId,
                    principalSchema: "public",
                    principalTable: "ProductTypes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ProductFiles",
            schema: "public",
            columns: table => new
            {
                Id = table.Column<long>(nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Version = table.Column<int>(nullable: false),
                Name = table.Column<string>(nullable: true),
                MimeType = table.Column<string>(nullable: true),
                FilePath = table.Column<string>(nullable: true),
                FileHash = table.Column<string>(nullable: true),
                ProductId = table.Column<long>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProductFiles", x => x.Id);
                table.ForeignKey(
                    name: "FK_ProductFiles_ProductTypes_ProductId",
                    column: x => x.ProductId,
                    principalSchema: "public",
                    principalTable: "ProductTypes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "ProductRecipes",
            schema: "public",
            columns: table => new
            {
                Id = table.Column<long>(nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Created = table.Column<DateTime>(nullable: false),
                Updated = table.Column<DateTime>(nullable: false),
                Deleted = table.Column<DateTime>(nullable: true),
                Type = table.Column<string>(nullable: true),
                Name = table.Column<string>(nullable: true),
                TemplateId = table.Column<long>(nullable: false),
                Revision = table.Column<int>(nullable: false),
                Classification = table.Column<int>(nullable: false),
                State = table.Column<int>(nullable: false),
                WorkplanId = table.Column<long>(nullable: true),
                ProductId = table.Column<long>(nullable: false),
                Integer1 = table.Column<long>(nullable: false),
                Integer2 = table.Column<long>(nullable: false),
                Integer3 = table.Column<long>(nullable: false),
                Integer4 = table.Column<long>(nullable: false),
                Integer5 = table.Column<long>(nullable: false),
                Integer6 = table.Column<long>(nullable: false),
                Integer7 = table.Column<long>(nullable: false),
                Integer8 = table.Column<long>(nullable: false),
                Float1 = table.Column<double>(nullable: false),
                Float2 = table.Column<double>(nullable: false),
                Float3 = table.Column<double>(nullable: false),
                Float4 = table.Column<double>(nullable: false),
                Float5 = table.Column<double>(nullable: false),
                Float6 = table.Column<double>(nullable: false),
                Float7 = table.Column<double>(nullable: false),
                Float8 = table.Column<double>(nullable: false),
                Text1 = table.Column<string>(nullable: true),
                Text2 = table.Column<string>(nullable: true),
                Text3 = table.Column<string>(nullable: true),
                Text4 = table.Column<string>(nullable: true),
                Text5 = table.Column<string>(nullable: true),
                Text6 = table.Column<string>(nullable: true),
                Text7 = table.Column<string>(nullable: true),
                Text8 = table.Column<string>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProductRecipes", x => x.Id);
                table.ForeignKey(
                    name: "FK_ProductRecipes_ProductTypes_ProductId",
                    column: x => x.ProductId,
                    principalSchema: "public",
                    principalTable: "ProductTypes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ProductRecipes_Workplans_WorkplanId",
                    column: x => x.WorkplanId,
                    principalSchema: "public",
                    principalTable: "Workplans",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "ProductTypeProperties",
            schema: "public",
            columns: table => new
            {
                Id = table.Column<long>(nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Created = table.Column<DateTime>(nullable: false),
                Updated = table.Column<DateTime>(nullable: false),
                Deleted = table.Column<DateTime>(nullable: true),
                State = table.Column<int>(nullable: false),
                ProductId = table.Column<long>(nullable: true),
                Integer1 = table.Column<long>(nullable: false),
                Integer2 = table.Column<long>(nullable: false),
                Integer3 = table.Column<long>(nullable: false),
                Integer4 = table.Column<long>(nullable: false),
                Integer5 = table.Column<long>(nullable: false),
                Integer6 = table.Column<long>(nullable: false),
                Integer7 = table.Column<long>(nullable: false),
                Integer8 = table.Column<long>(nullable: false),
                Float1 = table.Column<double>(nullable: false),
                Float2 = table.Column<double>(nullable: false),
                Float3 = table.Column<double>(nullable: false),
                Float4 = table.Column<double>(nullable: false),
                Float5 = table.Column<double>(nullable: false),
                Float6 = table.Column<double>(nullable: false),
                Float7 = table.Column<double>(nullable: false),
                Float8 = table.Column<double>(nullable: false),
                Text1 = table.Column<string>(nullable: true),
                Text2 = table.Column<string>(nullable: true),
                Text3 = table.Column<string>(nullable: true),
                Text4 = table.Column<string>(nullable: true),
                Text5 = table.Column<string>(nullable: true),
                Text6 = table.Column<string>(nullable: true),
                Text7 = table.Column<string>(nullable: true),
                Text8 = table.Column<string>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProductTypeProperties", x => x.Id);
                table.ForeignKey(
                    name: "FK_ProductTypeProperties_ProductTypes_ProductId",
                    column: x => x.ProductId,
                    principalSchema: "public",
                    principalTable: "ProductTypes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_PartLinks_ChildId",
            schema: "public",
            table: "PartLinks",
            column: "ChildId");

        migrationBuilder.CreateIndex(
            name: "IX_PartLinks_ParentId",
            schema: "public",
            table: "PartLinks",
            column: "ParentId");

        migrationBuilder.CreateIndex(
            name: "IX_ProductFiles_ProductId",
            schema: "public",
            table: "ProductFiles",
            column: "ProductId");

        migrationBuilder.CreateIndex(
            name: "IX_ProductInstances_ParentId",
            schema: "public",
            table: "ProductInstances",
            column: "ParentId");

        migrationBuilder.CreateIndex(
            name: "IX_ProductInstances_PartLinkEntityId",
            schema: "public",
            table: "ProductInstances",
            column: "PartLinkEntityId");

        migrationBuilder.CreateIndex(
            name: "IX_ProductInstances_ProductId",
            schema: "public",
            table: "ProductInstances",
            column: "ProductId");

        migrationBuilder.CreateIndex(
            name: "IX_ProductRecipes_ProductId",
            schema: "public",
            table: "ProductRecipes",
            column: "ProductId");

        migrationBuilder.CreateIndex(
            name: "IX_ProductRecipes_WorkplanId",
            schema: "public",
            table: "ProductRecipes",
            column: "WorkplanId");

        migrationBuilder.CreateIndex(
            name: "IX_ProductTypeProperties_ProductId",
            schema: "public",
            table: "ProductTypeProperties",
            column: "ProductId");

        migrationBuilder.CreateIndex(
            name: "IX_ProductTypes_CurrentVersionId",
            schema: "public",
            table: "ProductTypes",
            column: "CurrentVersionId");

        migrationBuilder.CreateIndex(
            name: "IX_ProductTypes_Identifier_Revision",
            schema: "public",
            table: "ProductTypes",
            columns: ["Identifier", "Revision"]);

        migrationBuilder.CreateIndex(
            name: "IX_WorkplanConnectorEntities_WorkplanId",
            schema: "public",
            table: "WorkplanConnectorEntities",
            column: "WorkplanId");

        migrationBuilder.CreateIndex(
            name: "IX_WorkplanConnectorReferences_ConnectorId",
            schema: "public",
            table: "WorkplanConnectorReferences",
            column: "ConnectorId");

        migrationBuilder.CreateIndex(
            name: "IX_WorkplanConnectorReferences_WorkplanStepId",
            schema: "public",
            table: "WorkplanConnectorReferences",
            column: "WorkplanStepId");

        migrationBuilder.CreateIndex(
            name: "IX_WorkplanOutputDescriptions_WorkplanStepId",
            schema: "public",
            table: "WorkplanOutputDescriptions",
            column: "WorkplanStepId");

        migrationBuilder.CreateIndex(
            name: "IX_WorkplanReferences_SourceId",
            schema: "public",
            table: "WorkplanReferences",
            column: "SourceId");

        migrationBuilder.CreateIndex(
            name: "IX_WorkplanReferences_TargetId",
            schema: "public",
            table: "WorkplanReferences",
            column: "TargetId");

        migrationBuilder.CreateIndex(
            name: "IX_WorkplanSteps_SubWorkplanId",
            schema: "public",
            table: "WorkplanSteps",
            column: "SubWorkplanId");

        migrationBuilder.CreateIndex(
            name: "IX_WorkplanSteps_WorkplanId",
            schema: "public",
            table: "WorkplanSteps",
            column: "WorkplanId");

        migrationBuilder.AddForeignKey(
            name: "FK_ProductInstances_ProductTypes_ProductId",
            schema: "public",
            table: "ProductInstances",
            column: "ProductId",
            principalSchema: "public",
            principalTable: "ProductTypes",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_ProductInstances_PartLinks_PartLinkEntityId",
            schema: "public",
            table: "ProductInstances",
            column: "PartLinkEntityId",
            principalSchema: "public",
            principalTable: "PartLinks",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_ProductTypes_ProductTypeProperties_CurrentVersionId",
            schema: "public",
            table: "ProductTypes",
            column: "CurrentVersionId",
            principalSchema: "public",
            principalTable: "ProductTypeProperties",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_ProductTypeProperties_ProductTypes_ProductId",
            schema: "public",
            table: "ProductTypeProperties");

        migrationBuilder.DropTable(
            name: "ProductFiles",
            schema: "public");

        migrationBuilder.DropTable(
            name: "ProductInstances",
            schema: "public");

        migrationBuilder.DropTable(
            name: "ProductRecipes",
            schema: "public");

        migrationBuilder.DropTable(
            name: "WorkplanConnectorReferences",
            schema: "public");

        migrationBuilder.DropTable(
            name: "WorkplanOutputDescriptions",
            schema: "public");

        migrationBuilder.DropTable(
            name: "WorkplanReferences",
            schema: "public");

        migrationBuilder.DropTable(
            name: "PartLinks",
            schema: "public");

        migrationBuilder.DropTable(
            name: "WorkplanConnectorEntities",
            schema: "public");

        migrationBuilder.DropTable(
            name: "WorkplanSteps",
            schema: "public");

        migrationBuilder.DropTable(
            name: "Workplans",
            schema: "public");

        migrationBuilder.DropTable(
            name: "ProductTypes",
            schema: "public");

        migrationBuilder.DropTable(
            name: "ProductTypeProperties",
            schema: "public");
    }
}
