// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Moryx.Products.Management.Model.Migrations.Sqlite;

/// <inheritdoc />
public partial class RemovedProductFiles : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ProductFiles",
            schema: "public");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ProductFiles",
            schema: "public",
            columns: table => new
            {
                Id = table.Column<long>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                ProductId = table.Column<long>(type: "INTEGER", nullable: true),
                FileHash = table.Column<string>(type: "TEXT", nullable: true),
                FilePath = table.Column<string>(type: "TEXT", nullable: true),
                MimeType = table.Column<string>(type: "TEXT", nullable: true),
                Name = table.Column<string>(type: "TEXT", nullable: true),
                Version = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProductFiles", x => x.Id);
                table.ForeignKey(
                    name: "FK_ProductFiles_ProductTypes_ProductId",
                    column: x => x.ProductId,
                    principalSchema: "public",
                    principalTable: "ProductTypes",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_ProductFiles_ProductId",
            schema: "public",
            table: "ProductFiles",
            column: "ProductId");
    }
}
