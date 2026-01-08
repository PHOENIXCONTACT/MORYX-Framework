// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore.Migrations;
using Moryx.AbstractionLayer.Products;

#nullable disable

namespace Moryx.Products.Management.Model.Migrations.Npgsql
{
    /// <inheritdoc />
    public partial class ProductState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "State",
                schema: "public",
                table: "ProductInstances",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.Sql($"UPDATE public.\"ProductInstances\" SET \"State\" = {(int)ProductInstanceState.Failure} WHERE \"State\" = 4;");
            migrationBuilder.Sql($"UPDATE public.\"ProductInstances\" SET \"State\" = {(int)ProductInstanceState.Success} WHERE \"State\" = 3;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"UPDATE public.\"ProductInstances\" SET \"State\" = 3 WHERE \"State\" =  {(int)ProductInstanceState.Success};");
            migrationBuilder.Sql($"UPDATE public.\"ProductInstances\" SET \"State\" = 4 WHERE \"State\" = {(int)ProductInstanceState.Failure};");

            migrationBuilder.AlterColumn<long>(
                name: "State",
                schema: "public",
                table: "ProductInstances",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
