// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Moryx.Resources.Management.Model.Migrations.Npgsql;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "public");

        migrationBuilder.CreateTable(
            name: "Resources",
            schema: "public",
            columns: table => new
            {
                Id = table.Column<long>(nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Created = table.Column<DateTime>(nullable: false),
                Updated = table.Column<DateTime>(nullable: false),
                Deleted = table.Column<DateTime>(nullable: true),
                Name = table.Column<string>(nullable: true),
                Description = table.Column<string>(nullable: true),
                ExtensionData = table.Column<string>(nullable: true),
                Type = table.Column<string>(nullable: true)
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
                Id = table.Column<long>(nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                RelationType = table.Column<int>(nullable: false),
                SourceName = table.Column<string>(nullable: true),
                SourceId = table.Column<long>(nullable: false),
                TargetName = table.Column<string>(nullable: true),
                TargetId = table.Column<long>(nullable: false)
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

    /// <inheritdoc />
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