// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moryx.Shifts.Management.Model.Migrations.Sqlite
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "ShiftTypeEntities",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    Endtime = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    Periode = table.Column<byte>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deleted = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftTypeEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShiftEntities",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    ShiftTypeId = table.Column<long>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deleted = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftEntities_ShiftTypeEntities_ShiftTypeId",
                        column: x => x.ShiftTypeId,
                        principalSchema: "public",
                        principalTable: "ShiftTypeEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftAssignementEntities",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ResourceId = table.Column<long>(type: "INTEGER", nullable: false),
                    OperatorIdentifier = table.Column<string>(type: "TEXT", nullable: false),
                    Note = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    AssignedDays = table.Column<int>(type: "INTEGER", nullable: false),
                    ShiftId = table.Column<long>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deleted = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftAssignementEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftAssignementEntities_ShiftEntities_ShiftId",
                        column: x => x.ShiftId,
                        principalSchema: "public",
                        principalTable: "ShiftEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftAssignementEntities_ShiftId",
                schema: "public",
                table: "ShiftAssignementEntities",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftEntities_ShiftTypeId",
                schema: "public",
                table: "ShiftEntities",
                column: "ShiftTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShiftAssignementEntities",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ShiftEntities",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ShiftTypeEntities",
                schema: "public");
        }
    }
}

