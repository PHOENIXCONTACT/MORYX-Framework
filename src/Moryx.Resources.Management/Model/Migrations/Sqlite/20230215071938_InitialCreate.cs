// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moryx.Resources.Management.Model.Migrations.Sqlite
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "Resources",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ExtensionData = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deleted = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResourceRelations",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RelationType = table.Column<int>(type: "INTEGER", nullable: false),
                    SourceName = table.Column<string>(type: "TEXT", nullable: true),
                    SourceId = table.Column<long>(type: "INTEGER", nullable: false),
                    TargetName = table.Column<string>(type: "TEXT", nullable: true),
                    TargetId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceRelations_Resources_SourceId",
                        column: x => x.SourceId,
                        principalSchema: "public",
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceRelations_Resources_TargetId",
                        column: x => x.TargetId,
                        principalSchema: "public",
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResourceRelations_SourceId",
                schema: "public",
                table: "ResourceRelations",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceRelations_TargetId",
                schema: "public",
                table: "ResourceRelations",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_Name",
                schema: "public",
                table: "Resources",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourceRelations",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Resources",
                schema: "public");
        }
    }
}
