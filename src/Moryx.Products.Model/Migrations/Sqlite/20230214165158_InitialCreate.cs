// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moryx.Products.Model.Migrations.Sqlite
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "Workplans",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxElementId = table.Column<int>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deleted = table.Column<DateTime>(type: "TEXT", nullable: true)
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
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ConnectorId = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Classification = table.Column<int>(type: "INTEGER", nullable: false),
                    WorkplanId = table.Column<long>(type: "INTEGER", nullable: false),
                    PositionX = table.Column<int>(type: "INTEGER", nullable: false),
                    PositionY = table.Column<int>(type: "INTEGER", nullable: false)
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
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReferenceType = table.Column<int>(type: "INTEGER", nullable: false),
                    SourceId = table.Column<long>(type: "INTEGER", nullable: false),
                    TargetId = table.Column<long>(type: "INTEGER", nullable: false)
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
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StepId = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Assembly = table.Column<string>(type: "TEXT", nullable: true),
                    NameSpace = table.Column<string>(type: "TEXT", nullable: true),
                    Classname = table.Column<string>(type: "TEXT", nullable: true),
                    Parameters = table.Column<string>(type: "TEXT", nullable: true),
                    WorkplanId = table.Column<long>(type: "INTEGER", nullable: false),
                    SubWorkplanId = table.Column<long>(type: "INTEGER", nullable: true),
                    PositionX = table.Column<int>(type: "INTEGER", nullable: false),
                    PositionY = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkplanSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkplanSteps_Workplans_SubWorkplanId",
                        column: x => x.SubWorkplanId,
                        principalSchema: "public",
                        principalTable: "Workplans",
                        principalColumn: "Id");
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
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Index = table.Column<int>(type: "INTEGER", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    ConnectorId = table.Column<long>(type: "INTEGER", nullable: true),
                    WorkplanStepId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkplanConnectorReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkplanConnectorReferences_WorkplanConnectorEntities_ConnectorId",
                        column: x => x.ConnectorId,
                        principalSchema: "public",
                        principalTable: "WorkplanConnectorEntities",
                        principalColumn: "Id");
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
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Index = table.Column<int>(type: "INTEGER", nullable: false),
                    OutputType = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    MappingValue = table.Column<long>(type: "INTEGER", nullable: false),
                    WorkplanStepId = table.Column<long>(type: "INTEGER", nullable: false)
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
                name: "PartLinks",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<long>(type: "INTEGER", nullable: false),
                    ChildId = table.Column<long>(type: "INTEGER", nullable: false),
                    PropertyName = table.Column<string>(type: "TEXT", nullable: true),
                    Integer1 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer2 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer3 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer4 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer5 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer6 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer7 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer8 = table.Column<long>(type: "INTEGER", nullable: false),
                    Float1 = table.Column<double>(type: "REAL", nullable: false),
                    Float2 = table.Column<double>(type: "REAL", nullable: false),
                    Float3 = table.Column<double>(type: "REAL", nullable: false),
                    Float4 = table.Column<double>(type: "REAL", nullable: false),
                    Float5 = table.Column<double>(type: "REAL", nullable: false),
                    Float6 = table.Column<double>(type: "REAL", nullable: false),
                    Float7 = table.Column<double>(type: "REAL", nullable: false),
                    Float8 = table.Column<double>(type: "REAL", nullable: false),
                    Text1 = table.Column<string>(type: "TEXT", nullable: true),
                    Text2 = table.Column<string>(type: "TEXT", nullable: true),
                    Text3 = table.Column<string>(type: "TEXT", nullable: true),
                    Text4 = table.Column<string>(type: "TEXT", nullable: true),
                    Text5 = table.Column<string>(type: "TEXT", nullable: true),
                    Text6 = table.Column<string>(type: "TEXT", nullable: true),
                    Text7 = table.Column<string>(type: "TEXT", nullable: true),
                    Text8 = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartLinks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductFiles",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    MimeType = table.Column<string>(type: "TEXT", nullable: true),
                    FilePath = table.Column<string>(type: "TEXT", nullable: true),
                    FileHash = table.Column<string>(type: "TEXT", nullable: true),
                    ProductId = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductInstances",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    State = table.Column<long>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<long>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<long>(type: "INTEGER", nullable: true),
                    PartLinkEntityId = table.Column<long>(type: "INTEGER", nullable: true),
                    Integer1 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer2 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer3 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer4 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer5 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer6 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer7 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer8 = table.Column<long>(type: "INTEGER", nullable: false),
                    Float1 = table.Column<double>(type: "REAL", nullable: false),
                    Float2 = table.Column<double>(type: "REAL", nullable: false),
                    Float3 = table.Column<double>(type: "REAL", nullable: false),
                    Float4 = table.Column<double>(type: "REAL", nullable: false),
                    Float5 = table.Column<double>(type: "REAL", nullable: false),
                    Float6 = table.Column<double>(type: "REAL", nullable: false),
                    Float7 = table.Column<double>(type: "REAL", nullable: false),
                    Float8 = table.Column<double>(type: "REAL", nullable: false),
                    Text1 = table.Column<string>(type: "TEXT", nullable: true),
                    Text2 = table.Column<string>(type: "TEXT", nullable: true),
                    Text3 = table.Column<string>(type: "TEXT", nullable: true),
                    Text4 = table.Column<string>(type: "TEXT", nullable: true),
                    Text5 = table.Column<string>(type: "TEXT", nullable: true),
                    Text6 = table.Column<string>(type: "TEXT", nullable: true),
                    Text7 = table.Column<string>(type: "TEXT", nullable: true),
                    Text8 = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductInstances_PartLinks_PartLinkEntityId",
                        column: x => x.PartLinkEntityId,
                        principalSchema: "public",
                        principalTable: "PartLinks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductInstances_ProductInstances_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "public",
                        principalTable: "ProductInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductRecipes",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    TemplateId = table.Column<long>(type: "INTEGER", nullable: false),
                    Revision = table.Column<int>(type: "INTEGER", nullable: false),
                    Classification = table.Column<int>(type: "INTEGER", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    WorkplanId = table.Column<long>(type: "INTEGER", nullable: true),
                    ProductId = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer1 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer2 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer3 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer4 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer5 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer6 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer7 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer8 = table.Column<long>(type: "INTEGER", nullable: false),
                    Float1 = table.Column<double>(type: "REAL", nullable: false),
                    Float2 = table.Column<double>(type: "REAL", nullable: false),
                    Float3 = table.Column<double>(type: "REAL", nullable: false),
                    Float4 = table.Column<double>(type: "REAL", nullable: false),
                    Float5 = table.Column<double>(type: "REAL", nullable: false),
                    Float6 = table.Column<double>(type: "REAL", nullable: false),
                    Float7 = table.Column<double>(type: "REAL", nullable: false),
                    Float8 = table.Column<double>(type: "REAL", nullable: false),
                    Text1 = table.Column<string>(type: "TEXT", nullable: true),
                    Text2 = table.Column<string>(type: "TEXT", nullable: true),
                    Text3 = table.Column<string>(type: "TEXT", nullable: true),
                    Text4 = table.Column<string>(type: "TEXT", nullable: true),
                    Text5 = table.Column<string>(type: "TEXT", nullable: true),
                    Text6 = table.Column<string>(type: "TEXT", nullable: true),
                    Text7 = table.Column<string>(type: "TEXT", nullable: true),
                    Text8 = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deleted = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRecipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductRecipes_Workplans_WorkplanId",
                        column: x => x.WorkplanId,
                        principalSchema: "public",
                        principalTable: "Workplans",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductTypeProperties",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<long>(type: "INTEGER", nullable: true),
                    Integer1 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer2 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer3 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer4 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer5 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer6 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer7 = table.Column<long>(type: "INTEGER", nullable: false),
                    Integer8 = table.Column<long>(type: "INTEGER", nullable: false),
                    Float1 = table.Column<double>(type: "REAL", nullable: false),
                    Float2 = table.Column<double>(type: "REAL", nullable: false),
                    Float3 = table.Column<double>(type: "REAL", nullable: false),
                    Float4 = table.Column<double>(type: "REAL", nullable: false),
                    Float5 = table.Column<double>(type: "REAL", nullable: false),
                    Float6 = table.Column<double>(type: "REAL", nullable: false),
                    Float7 = table.Column<double>(type: "REAL", nullable: false),
                    Float8 = table.Column<double>(type: "REAL", nullable: false),
                    Text1 = table.Column<string>(type: "TEXT", nullable: true),
                    Text2 = table.Column<string>(type: "TEXT", nullable: true),
                    Text3 = table.Column<string>(type: "TEXT", nullable: true),
                    Text4 = table.Column<string>(type: "TEXT", nullable: true),
                    Text5 = table.Column<string>(type: "TEXT", nullable: true),
                    Text6 = table.Column<string>(type: "TEXT", nullable: true),
                    Text7 = table.Column<string>(type: "TEXT", nullable: true),
                    Text8 = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deleted = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTypeProperties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductTypes",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TypeName = table.Column<string>(type: "TEXT", nullable: true),
                    Identifier = table.Column<string>(type: "TEXT", nullable: true),
                    Revision = table.Column<short>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentVersionId = table.Column<long>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deleted = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductTypes_ProductTypeProperties_CurrentVersionId",
                        column: x => x.CurrentVersionId,
                        principalSchema: "public",
                        principalTable: "ProductTypeProperties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "FK_PartLinks_ProductTypes_ChildId",
                schema: "public",
                table: "PartLinks",
                column: "ChildId",
                principalSchema: "public",
                principalTable: "ProductTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartLinks_ProductTypes_ParentId",
                schema: "public",
                table: "PartLinks",
                column: "ParentId",
                principalSchema: "public",
                principalTable: "ProductTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFiles_ProductTypes_ProductId",
                schema: "public",
                table: "ProductFiles",
                column: "ProductId",
                principalSchema: "public",
                principalTable: "ProductTypes",
                principalColumn: "Id");

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
                name: "FK_ProductRecipes_ProductTypes_ProductId",
                schema: "public",
                table: "ProductRecipes",
                column: "ProductId",
                principalSchema: "public",
                principalTable: "ProductTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTypeProperties_ProductTypes_ProductId",
                schema: "public",
                table: "ProductTypeProperties",
                column: "ProductId",
                principalSchema: "public",
                principalTable: "ProductTypes",
                principalColumn: "Id");
        }

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
}
