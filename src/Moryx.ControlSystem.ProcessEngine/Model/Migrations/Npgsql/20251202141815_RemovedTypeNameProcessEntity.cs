// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moryx.ControlSystem.ProcessEngine.Model.Migrations.Npgsql;

/// <inheritdoc />
public partial class RemovedTypeNameProcessEntity : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "TypeName",
            schema: "public",
            table: "Processes");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "TypeName",
            schema: "public",
            table: "Processes",
            type: "text",
            nullable: true);
    }
}