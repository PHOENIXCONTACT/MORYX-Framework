// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Moryx.Notifications.Publisher.Model.Migrations.Npgsql;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "public");

        migrationBuilder.CreateTable(
            name: "NotificationTypeEntities",
            schema: "public",
            columns: table => new
            {
                Id = table.Column<long>(nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Type = table.Column<string>(nullable: true),
                Severity = table.Column<int>(nullable: false),
                Identifier = table.Column<string>(nullable: true),
                ExtensionData = table.Column<string>(nullable: true),
                IsDisabled = table.Column<bool>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_NotificationTypeEntities", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "NotificationEntities",
            schema: "public",
            columns: table => new
            {
                Id = table.Column<long>(nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Identifier = table.Column<Guid>(nullable: false),
                Source = table.Column<string>(nullable: true),
                Sender = table.Column<string>(nullable: true),
                Title = table.Column<string>(nullable: true),
                Message = table.Column<string>(nullable: true),
                Acknowledged = table.Column<DateTime>(nullable: true),
                ExtensionData = table.Column<string>(nullable: true),
                Created = table.Column<DateTime>(nullable: false),
                TypeId = table.Column<long>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_NotificationEntities", x => x.Id);
                table.ForeignKey(
                    name: "FK_NotificationEntities_NotificationTypeEntities_TypeId",
                    column: x => x.TypeId,
                    principalSchema: "public",
                    principalTable: "NotificationTypeEntities",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_NotificationEntities_TypeId",
            schema: "public",
            table: "NotificationEntities",
            column: "TypeId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "NotificationEntities",
            schema: "public");

        migrationBuilder.DropTable(
            name: "NotificationTypeEntities",
            schema: "public");
    }
}