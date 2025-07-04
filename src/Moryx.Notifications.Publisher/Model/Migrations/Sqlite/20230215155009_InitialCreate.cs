﻿#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moryx.Notifications.Publisher.Model.Migrations.Sqlite
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "NotificationTypeEntities",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    Severity = table.Column<int>(type: "INTEGER", nullable: false),
                    Identifier = table.Column<string>(type: "TEXT", nullable: true),
                    ExtensionData = table.Column<string>(type: "TEXT", nullable: true),
                    IsDisabled = table.Column<bool>(type: "INTEGER", nullable: false)
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
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Identifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: true),
                    Sender = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Message = table.Column<string>(type: "TEXT", nullable: true),
                    Acknowledged = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExtensionData = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TypeId = table.Column<long>(type: "INTEGER", nullable: false)
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
}
