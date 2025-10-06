// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Moryx.ControlSystem.ProcessEngine.Model.Migrations.Npgsql
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
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: false),
                    Deleted = table.Column<DateTime>(nullable: true),
                    RecipeId = table.Column<long>(nullable: false),
                    RecipeProvider = table.Column<string>(nullable: true),
                    Amount = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    PreviousId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobEntities_JobEntities_PreviousId",
                        column: x => x.PreviousId,
                        principalSchema: "public",
                        principalTable: "JobEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TracingTypes",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Assembly = table.Column<string>(nullable: true),
                    NameSpace = table.Column<string>(nullable: true),
                    Classname = table.Column<string>(nullable: true)
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
                    Id = table.Column<long>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: false),
                    Deleted = table.Column<DateTime>(nullable: true),
                    TypeName = table.Column<string>(nullable: true),
                    ReferenceId = table.Column<long>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    Rework = table.Column<bool>(nullable: false),
                    JobId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessEntities_JobEntities_JobId",
                        column: x => x.JobId,
                        principalSchema: "public",
                        principalTable: "JobEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ActivityEntities",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    TaskId = table.Column<long>(nullable: false),
                    ResourceId = table.Column<long>(nullable: false),
                    Started = table.Column<DateTime>(nullable: true),
                    Completed = table.Column<DateTime>(nullable: true),
                    ProcessHolderId = table.Column<long>(nullable: true),
                    TracingText = table.Column<string>(nullable: true),
                    Progress = table.Column<int>(nullable: false),
                    TracingData = table.Column<string>(nullable: true),
                    Success = table.Column<bool>(nullable: false),
                    Result = table.Column<long>(nullable: true),
                    ProcessId = table.Column<long>(nullable: false),
                    JobId = table.Column<long>(nullable: true),
                    TracingTypeId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityEntities_JobEntities_JobId",
                        column: x => x.JobId,
                        principalSchema: "public",
                        principalTable: "JobEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
            name: "TokenHolderEntities",
            schema: "public",
            columns: table => new
            {
                Id = table.Column<long>(nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                HolderId = table.Column<long>(nullable: false),
                Tokens = table.Column<string>(nullable: true),
                ProcessId = table.Column<long>(nullable: false)
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

