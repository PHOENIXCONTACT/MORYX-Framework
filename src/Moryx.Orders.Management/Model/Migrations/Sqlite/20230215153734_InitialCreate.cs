// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moryx.Orders.Management.Model.Migrations.Sqlite
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
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Number = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deleted = table.Column<DateTime>(type: "TEXT", nullable: true)
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
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<long>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<long>(type: "INTEGER", nullable: false),
                    Identifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    Number = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    AssignState = table.Column<int>(type: "INTEGER", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalAmount = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetAmount = table.Column<int>(type: "INTEGER", nullable: false),
                    OverDeliveryAmount = table.Column<int>(type: "INTEGER", nullable: false),
                    UnderDeliveryAmount = table.Column<int>(type: "INTEGER", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: true),
                    TargetCycleTime = table.Column<double>(type: "REAL", nullable: false),
                    PlannedStart = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PlannedEnd = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ActualStart = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ActualEnd = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TargetStock = table.Column<string>(type: "TEXT", nullable: true),
                    Unit = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deleted = table.Column<DateTime>(type: "TEXT", nullable: true)
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
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LoadingEquipment = table.Column<string>(type: "TEXT", nullable: true),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false),
                    ToteBoxNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PartId = table.Column<long>(type: "INTEGER", nullable: true),
                    OperationId = table.Column<long>(type: "INTEGER", nullable: false)
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
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JobId = table.Column<long>(type: "INTEGER", nullable: false),
                    OperationId = table.Column<long>(type: "INTEGER", nullable: false)
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
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RecipeId = table.Column<long>(type: "INTEGER", nullable: false),
                    OperationId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationRecipeReferenceEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationRecipeReferenceEntities_OperationEntities_OperationId",
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
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OperationId = table.Column<long>(type: "INTEGER", nullable: false),
                    ConfirmationType = table.Column<int>(type: "INTEGER", nullable: false),
                    SuccessCount = table.Column<int>(type: "INTEGER", nullable: false),
                    FailureCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: true),
                    ReportedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserIdentifier = table.Column<string>(type: "TEXT", nullable: true)
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
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Number = table.Column<string>(type: "TEXT", nullable: true),
                    Quantity = table.Column<double>(type: "REAL", nullable: false),
                    Unit = table.Column<string>(type: "TEXT", nullable: true),
                    Classification = table.Column<int>(type: "INTEGER", nullable: false),
                    StagingIndicator = table.Column<int>(type: "INTEGER", nullable: false),
                    OperationId = table.Column<long>(type: "INTEGER", nullable: false)
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

