// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moryx.Products.Management.Model.Migrations.Npgsql
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
                type: "text",
                nullable: true);

            migrationBuilder.Sql("UPDATE public.\"WorkplanSteps\" SET \"TypeName\" = \"NameSpace\" || '.' || \"ClassName\";");

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
                name: "NameSpace",
                schema: "public",
                table: "WorkplanSteps",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Assembly",
                schema: "public",
                table: "WorkplanSteps",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Classname",
                schema: "public",
                table: "WorkplanSteps",
                type: "text",
                nullable: true);
        }
    }
}
