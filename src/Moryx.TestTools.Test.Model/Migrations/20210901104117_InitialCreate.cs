// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Moryx.TestTools.Test.Model.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "Cars",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: false),
                    Deleted = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Price = table.Column<int>(nullable: false),
                    Image = table.Column<byte[]>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    Performance = table.Column<int>(nullable: true)
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
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: false),
                    Deleted = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Size = table.Column<int>(nullable: false),
                    IsMethLabratory = table.Column<bool>(nullable: false),
                    IsBurnedDown = table.Column<bool>(nullable: false),
                    ToRent = table.Column<bool>(nullable: false)
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
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Float1 = table.Column<double>(nullable: false),
                    Name1 = table.Column<string>(nullable: true),
                    Number1 = table.Column<int>(nullable: false),
                    Float2 = table.Column<double>(nullable: false),
                    Name2 = table.Column<string>(nullable: true),
                    Number2 = table.Column<int>(nullable: false),
                    Float3 = table.Column<double>(nullable: false),
                    Name3 = table.Column<string>(nullable: true),
                    Number3 = table.Column<int>(nullable: false),
                    Float4 = table.Column<double>(nullable: false),
                    Name4 = table.Column<string>(nullable: true),
                    Number4 = table.Column<int>(nullable: false),
                    Float5 = table.Column<double>(nullable: false),
                    Name5 = table.Column<string>(nullable: true),
                    Number5 = table.Column<int>(nullable: false)
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
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JsonData = table.Column<string>(nullable: true)
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
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CarId = table.Column<long>(nullable: true),
                    WheelType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wheels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wheels_Cars_CarId",
                        column: x => x.CarId,
                        principalSchema: "public",
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wheels_CarId",
                schema: "public",
                table: "Wheels",
                column: "CarId");
        }

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

