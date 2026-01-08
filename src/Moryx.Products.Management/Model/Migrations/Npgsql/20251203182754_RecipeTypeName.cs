// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moryx.Products.Management.Model.Migrations.Npgsql;

/// <inheritdoc />
public partial class RecipeTypeName : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Type",
            schema: "public",
            table: "ProductRecipes",
            newName: "TypeName");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "TypeName",
            schema: "public",
            table: "ProductRecipes",
            newName: "Type");
    }
}