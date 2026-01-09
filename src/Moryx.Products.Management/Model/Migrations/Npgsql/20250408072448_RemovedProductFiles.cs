// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Moryx.Products.Management.Model.Migrations.Npgsql;

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
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ProductId = table.Column<long>(type: "bigint", nullable: true),
                FileHash = table.Column<string>(type: "text", nullable: true),
                FilePath = table.Column<string>(type: "text", nullable: true),
                MimeType = table.Column<string>(type: "text", nullable: true),
                Name = table.Column<string>(type: "text", nullable: true),
                Version = table.Column<int>(type: "integer", nullable: false)
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
