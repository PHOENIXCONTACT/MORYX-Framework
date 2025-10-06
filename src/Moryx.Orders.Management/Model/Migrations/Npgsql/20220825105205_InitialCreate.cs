// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Moryx.Orders.Management.Model.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "OrderEntities",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: false),
                    Deleted = table.Column<DateTime>(nullable: true),
                    Number = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperationEntities",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: false),
                    Deleted = table.Column<DateTime>(nullable: true),
                    OrderId = table.Column<long>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    Identifier = table.Column<Guid>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    AssignState = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TotalAmount = table.Column<int>(nullable: false),
                    TargetAmount = table.Column<int>(nullable: false),
                    OverDeliveryAmount = table.Column<int>(nullable: false),
                    UnderDeliveryAmount = table.Column<int>(nullable: false),
                    Source = table.Column<string>(nullable: true),
                    TargetCycleTime = table.Column<double>(nullable: false),
                    PlannedStart = table.Column<DateTime>(nullable: false),
                    PlannedEnd = table.Column<DateTime>(nullable: false),
                    ActualStart = table.Column<DateTime>(nullable: true),
                    ActualEnd = table.Column<DateTime>(nullable: true),
                    TargetStock = table.Column<string>(nullable: true),
                    Unit = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationEntities_OrderEntities_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "public",
                        principalTable: "OrderEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperationAdviceEntities",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LoadingEquipment = table.Column<string>(nullable: true),
                    Amount = table.Column<int>(nullable: false),
                    ToteBoxNumber = table.Column<string>(nullable: true),
                    PartId = table.Column<long>(nullable: true),
                    OperationId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationAdviceEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationAdviceEntities_OperationEntities_OperationId",
                        column: x => x.OperationId,
                        principalSchema: "public",
                        principalTable: "OperationEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperationJobReferenceEntities",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobId = table.Column<long>(nullable: false),
                    OperationId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationJobReferenceEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationJobReferenceEntities_OperationEntities_OperationId",
                        column: x => x.OperationId,
                        principalSchema: "public",
                        principalTable: "OperationEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperationRecipeReferenceEntities",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecipeId = table.Column<long>(nullable: false),
                    OperationId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationRecipeReferenceEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationRecipeReferenceEntities_OperationEntities_Operatio~",
                        column: x => x.OperationId,
                        principalSchema: "public",
                        principalTable: "OperationEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperationReportEntities",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OperationId = table.Column<long>(nullable: false),
                    ConfirmationType = table.Column<int>(nullable: false),
                    SuccessCount = table.Column<int>(nullable: false),
                    FailureCount = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    ReportedDate = table.Column<DateTime>(nullable: false),
                    UserIdentifier = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationReportEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationReportEntities_OperationEntities_OperationId",
                        column: x => x.OperationId,
                        principalSchema: "public",
                        principalTable: "OperationEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductPartEntities",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Number = table.Column<string>(nullable: true),
                    Quantity = table.Column<double>(nullable: false),
                    Unit = table.Column<string>(nullable: true),
                    Classification = table.Column<int>(nullable: false),
                    StagingIndicator = table.Column<int>(nullable: false),
                    OperationId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPartEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPartEntities_OperationEntities_OperationId",
                        column: x => x.OperationId,
                        principalSchema: "public",
                        principalTable: "OperationEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OperationAdviceEntities_OperationId",
                schema: "public",
                table: "OperationAdviceEntities",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationEntities_OrderId",
                schema: "public",
                table: "OperationEntities",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationJobReferenceEntities_OperationId",
                schema: "public",
                table: "OperationJobReferenceEntities",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationRecipeReferenceEntities_OperationId",
                schema: "public",
                table: "OperationRecipeReferenceEntities",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationReportEntities_OperationId",
                schema: "public",
                table: "OperationReportEntities",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPartEntities_OperationId",
                schema: "public",
                table: "ProductPartEntities",
                column: "OperationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperationAdviceEntities",
                schema: "public");

            migrationBuilder.DropTable(
                name: "OperationJobReferenceEntities",
                schema: "public");

            migrationBuilder.DropTable(
                name: "OperationRecipeReferenceEntities",
                schema: "public");

            migrationBuilder.DropTable(
                name: "OperationReportEntities",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ProductPartEntities",
                schema: "public");

            migrationBuilder.DropTable(
                name: "OperationEntities",
                schema: "public");

            migrationBuilder.DropTable(
                name: "OrderEntities",
                schema: "public");
        }
    }
}

