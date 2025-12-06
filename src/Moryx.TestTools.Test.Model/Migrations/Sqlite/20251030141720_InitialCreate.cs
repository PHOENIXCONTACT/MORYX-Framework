// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moryx.TestTools.Test.Model.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "Cars",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Price = table.Column<int>(type: "INTEGER", nullable: false),
                    Image = table.Column<byte[]>(type: "BLOB", nullable: true),
                    ReleaseDateLocal = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReleaseDateUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 21, nullable: false),
                    Performance = table.Column<int>(type: "INTEGER", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deleted = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Houses",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Size = table.Column<int>(type: "INTEGER", nullable: false),
                    IsMethLabratory = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsBurnedDown = table.Column<bool>(type: "INTEGER", nullable: false),
                    ToRent = table.Column<bool>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deleted = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Houses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HugePocos",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Float1 = table.Column<double>(type: "REAL", nullable: false),
                    Name1 = table.Column<string>(type: "TEXT", nullable: true),
                    Number1 = table.Column<int>(type: "INTEGER", nullable: false),
                    Float2 = table.Column<double>(type: "REAL", nullable: false),
                    Name2 = table.Column<string>(type: "TEXT", nullable: true),
                    Number2 = table.Column<int>(type: "INTEGER", nullable: false),
                    Float3 = table.Column<double>(type: "REAL", nullable: false),
                    Name3 = table.Column<string>(type: "TEXT", nullable: true),
                    Number3 = table.Column<int>(type: "INTEGER", nullable: false),
                    Float4 = table.Column<double>(type: "REAL", nullable: false),
                    Name4 = table.Column<string>(type: "TEXT", nullable: true),
                    Number4 = table.Column<int>(type: "INTEGER", nullable: false),
                    Float5 = table.Column<double>(type: "REAL", nullable: false),
                    Name5 = table.Column<string>(type: "TEXT", nullable: true),
                    Number5 = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HugePocos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jsons",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JsonData = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jsons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wheels",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CarId = table.Column<long>(type: "INTEGER", nullable: true),
                    WheelType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wheels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wheels_Cars_CarId",
                        column: x => x.CarId,
                        principalSchema: "public",
                        principalTable: "Cars",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wheels_CarId",
                schema: "public",
                table: "Wheels",
                column: "CarId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Houses",
                schema: "public");

            migrationBuilder.DropTable(
                name: "HugePocos",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Jsons",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Wheels",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Cars",
                schema: "public");
        }
    }
}
