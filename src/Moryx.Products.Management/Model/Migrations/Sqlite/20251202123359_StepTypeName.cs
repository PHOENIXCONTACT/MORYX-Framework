// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moryx.Products.Management.Model.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class StepTypeName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TypeName",
                schema: "public",
                table: "WorkplanSteps",
                type: "TEXT",
                nullable: true);

            migrationBuilder.Sql("UPDATE WorkplanSteps SET TypeName = NameSpace || '.' || Classname;");

            migrationBuilder.DropColumn(
                name: "Assembly",
                schema: "public",
                table: "WorkplanSteps");

            migrationBuilder.DropColumn(
                name: "Classname",
                schema: "public",
                table: "WorkplanSteps");

            migrationBuilder.DropColumn(
                name: "NameSpace",
                schema: "public",
                table: "WorkplanSteps");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TypeName",
                schema: "public",
                table: "WorkplanSteps",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Assembly",
                schema: "public",
                table: "WorkplanSteps",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Classname",
                schema: "public",
                table: "WorkplanSteps",
                type: "TEXT",
                nullable: true);
        }
    }
}
