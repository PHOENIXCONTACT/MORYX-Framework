// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Moryx.TestTools.Test.Model.Migrations.Npgsql
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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    Image = table.Column<byte[]>(type: "bytea", nullable: true),
                    ReleaseDateLocal = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReleaseDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false),
                    Performance = table.Column<int>(type: "integer", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Size = table.Column<int>(type: "integer", nullable: false),
                    IsMethLabratory = table.Column<bool>(type: "boolean", nullable: false),
                    IsBurnedDown = table.Column<bool>(type: "boolean", nullable: false),
                    ToRent = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Float1 = table.Column<double>(type: "double precision", nullable: false),
                    Name1 = table.Column<string>(type: "text", nullable: true),
                    Number1 = table.Column<int>(type: "integer", nullable: false),
                    Float2 = table.Column<double>(type: "double precision", nullable: false),
                    Name2 = table.Column<string>(type: "text", nullable: true),
                    Number2 = table.Column<int>(type: "integer", nullable: false),
                    Float3 = table.Column<double>(type: "double precision", nullable: false),
                    Name3 = table.Column<string>(type: "text", nullable: true),
                    Number3 = table.Column<int>(type: "integer", nullable: false),
                    Float4 = table.Column<double>(type: "double precision", nullable: false),
                    Name4 = table.Column<string>(type: "text", nullable: true),
                    Number4 = table.Column<int>(type: "integer", nullable: false),
                    Float5 = table.Column<double>(type: "double precision", nullable: false),
                    Name5 = table.Column<string>(type: "text", nullable: true),
                    Number5 = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JsonData = table.Column<string>(type: "text", nullable: true)
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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CarId = table.Column<long>(type: "bigint", nullable: true),
                    WheelType = table.Column<int>(type: "integer", nullable: false)
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
