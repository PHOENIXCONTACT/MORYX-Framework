// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moryx.ControlSystem.ProcessEngine.Model.Migrations.Sqlite
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "JobEntities",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RecipeId = table.Column<long>(type: "INTEGER", nullable: false),
                    RecipeProvider = table.Column<string>(type: "TEXT", nullable: true),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    PreviousId = table.Column<long>(type: "INTEGER", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deleted = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobEntities_JobEntities_PreviousId",
                        column: x => x.PreviousId,
                        principalSchema: "public",
                        principalTable: "JobEntities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TracingTypes",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Assembly = table.Column<string>(type: "TEXT", nullable: true),
                    NameSpace = table.Column<string>(type: "TEXT", nullable: true),
                    Classname = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TracingTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessEntities",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    TypeName = table.Column<string>(type: "TEXT", nullable: true),
                    ReferenceId = table.Column<long>(type: "INTEGER", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    Rework = table.Column<bool>(type: "INTEGER", nullable: false),
                    JobId = table.Column<long>(type: "INTEGER", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deleted = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessEntities_JobEntities_JobId",
                        column: x => x.JobId,
                        principalSchema: "public",
                        principalTable: "JobEntities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ActivityEntities",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    TaskId = table.Column<long>(type: "INTEGER", nullable: false),
                    ResourceId = table.Column<long>(type: "INTEGER", nullable: false),
                    Started = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Completed = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ProcessHolderId = table.Column<long>(type: "INTEGER", nullable: true),
                    TracingText = table.Column<string>(type: "TEXT", nullable: true),
                    Progress = table.Column<int>(type: "INTEGER", nullable: false),
                    TracingData = table.Column<string>(type: "TEXT", nullable: true),
                    Success = table.Column<bool>(type: "INTEGER", nullable: false),
                    Result = table.Column<long>(type: "INTEGER", nullable: true),
                    ProcessId = table.Column<long>(type: "INTEGER", nullable: false),
                    JobId = table.Column<long>(type: "INTEGER", nullable: true),
                    TracingTypeId = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityEntities_JobEntities_JobId",
                        column: x => x.JobId,
                        principalSchema: "public",
                        principalTable: "JobEntities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActivityEntities_ProcessEntities_ProcessId",
                        column: x => x.ProcessId,
                        principalSchema: "public",
                        principalTable: "ProcessEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityEntities_TracingTypes_TracingTypeId",
                        column: x => x.TracingTypeId,
                        principalSchema: "public",
                        principalTable: "TracingTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TokenHolderEntities",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HolderId = table.Column<long>(type: "INTEGER", nullable: false),
                    Tokens = table.Column<string>(type: "TEXT", nullable: true),
                    ProcessId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenHolderEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TokenHolderEntities_ProcessEntities_ProcessId",
                        column: x => x.ProcessId,
                        principalSchema: "public",
                        principalTable: "ProcessEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityEntities_JobId",
                schema: "public",
                table: "ActivityEntities",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityEntities_ProcessId",
                schema: "public",
                table: "ActivityEntities",
                column: "ProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityEntities_TracingTypeId",
                schema: "public",
                table: "ActivityEntities",
                column: "TracingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_JobEntities_PreviousId",
                schema: "public",
                table: "JobEntities",
                column: "PreviousId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessEntities_JobId",
                schema: "public",
                table: "ProcessEntities",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenHolderEntities_ProcessId",
                schema: "public",
                table: "TokenHolderEntities",
                column: "ProcessId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityEntities",
                schema: "public");

            migrationBuilder.DropTable(
                name: "TokenHolderEntities",
                schema: "public");

            migrationBuilder.DropTable(
                name: "TracingTypes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ProcessEntities",
                schema: "public");

            migrationBuilder.DropTable(
                name: "JobEntities",
                schema: "public");
        }
    }
}

