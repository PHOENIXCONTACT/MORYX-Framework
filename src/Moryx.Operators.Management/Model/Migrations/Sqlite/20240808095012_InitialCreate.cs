// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Moryx.Operators.Management.Model.Migrations.Sqlite;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "public");

        migrationBuilder.CreateTable(
            name: "OperatorEntities",
            schema: "public",
            columns: table => new
            {
                Id = table.Column<long>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Identifier = table.Column<string>(type: "TEXT", nullable: false),
                FirstName = table.Column<string>(type: "TEXT", nullable: true),
                LastName = table.Column<string>(type: "TEXT", nullable: true),
                Pseudonym = table.Column<string>(type: "TEXT", nullable: true),
                Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                Deleted = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OperatorEntities", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "SkillTypeEntities",
            schema: "public",
            columns: table => new
            {
                Id = table.Column<long>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                Duration = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                CapabilitiesType = table.Column<string>(type: "TEXT", nullable: false),
                CapabilitiesData = table.Column<string>(type: "TEXT", nullable: false),
                Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                Deleted = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SkillTypeEntities", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ResourceLinkEntities",
            schema: "public",
            columns: table => new
            {
                Id = table.Column<long>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                ResourceId = table.Column<long>(type: "INTEGER", nullable: false),
                OperatorEntityId = table.Column<long>(type: "INTEGER", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ResourceLinkEntities", x => x.Id);
                table.ForeignKey(
                    name: "FK_ResourceLinkEntities_OperatorEntities_OperatorEntityId",
                    column: x => x.OperatorEntityId,
                    principalSchema: "public",
                    principalTable: "OperatorEntities",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "SkillEntities",
            schema: "public",
            columns: table => new
            {
                Id = table.Column<long>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                ObtainedOn = table.Column<DateOnly>(type: "TEXT", nullable: false),
                Expiration = table.Column<DateOnly>(type: "TEXT", nullable: false),
                OperatorIdentifier = table.Column<string>(type: "TEXT", nullable: false),
                SkillTypeId = table.Column<long>(type: "INTEGER", nullable: false),
                Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                Deleted = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SkillEntities", x => x.Id);
                table.ForeignKey(
                    name: "FK_SkillEntities_SkillTypeEntities_SkillTypeId",
                    column: x => x.SkillTypeId,
                    principalSchema: "public",
                    principalTable: "SkillTypeEntities",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ResourceLinkEntities_OperatorEntityId",
            schema: "public",
            table: "ResourceLinkEntities",
            column: "OperatorEntityId");

        migrationBuilder.CreateIndex(
            name: "IX_SkillEntities_SkillTypeId",
            schema: "public",
            table: "SkillEntities",
            column: "SkillTypeId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ResourceLinkEntities",
            schema: "public");

        migrationBuilder.DropTable(
            name: "SkillEntities",
            schema: "public");

        migrationBuilder.DropTable(
            name: "OperatorEntities",
            schema: "public");

        migrationBuilder.DropTable(
            name: "SkillTypeEntities",
            schema: "public");
    }
}

