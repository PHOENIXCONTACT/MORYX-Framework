// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Moryx.Operators.Management.Model.Migrations.Npgsql;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "public");

        migrationBuilder.CreateTable(
            name: "OperatorEntities",
            schema: "public",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Identifier = table.Column<string>(type: "text", nullable: false),
                FirstName = table.Column<string>(type: "text", nullable: true),
                LastName = table.Column<string>(type: "text", nullable: true),
                Pseudonym = table.Column<string>(type: "text", nullable: true),
                Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                CapabilitiesType = table.Column<string>(type: "text", nullable: false),
                CapabilitiesData = table.Column<string>(type: "text", nullable: false),
                Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ResourceId = table.Column<long>(type: "bigint", nullable: false),
                OperatorEntityId = table.Column<long>(type: "bigint", nullable: true)
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
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ObtainedOn = table.Column<DateOnly>(type: "date", nullable: false),
                Expiration = table.Column<DateOnly>(type: "date", nullable: false),
                OperatorIdentifier = table.Column<string>(type: "text", nullable: false),
                SkillTypeId = table.Column<long>(type: "bigint", nullable: false),
                Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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

