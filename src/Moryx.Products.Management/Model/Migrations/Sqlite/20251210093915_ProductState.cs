// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore.Migrations;
using Moryx.AbstractionLayer.Products;

#nullable disable

namespace Moryx.Products.Management.Model.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class ProductState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"UPDATE ProductInstances SET State = {(int)ProductInstanceState.Failure} WHERE State = 4;");
            migrationBuilder.Sql($"UPDATE ProductInstances SET State = {(int)ProductInstanceState.Success} WHERE State = 3;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"UPDATE ProductInstances SET State = 3 WHERE State = {(int)ProductInstanceState.Success};");
            migrationBuilder.Sql($"UPDATE ProductInstances SET State = 4 WHERE State = {(int)ProductInstanceState.Failure};");
        }
    }
}
