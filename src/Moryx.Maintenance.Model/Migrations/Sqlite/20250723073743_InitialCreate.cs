using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moryx.Maintenance.Management.Model.Migrations.Sqlite
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
                name: "MaintenanceOrders",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ResourceId = table.Column<long>(type: "INTEGER", nullable: false),
                    IntervalType = table.Column<string>(type: "TEXT", nullable: true),
                    IntervalData = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Block = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deleted = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Acknowledgements",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OperatorId = table.Column<long>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    MaintenanceOrdersId = table.Column<long>(type: "INTEGER", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deleted = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Acknowledgements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Acknowledgements_MaintenanceOrders_MaintenanceOrdersId",
                        column: x => x.MaintenanceOrdersId,
                        principalSchema: "public",
                        principalTable: "MaintenanceOrders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VisualInstruction",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    Preview = table.Column<string>(type: "TEXT", nullable: true),
                    MaintenanceOrderId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisualInstruction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisualInstruction_MaintenanceOrders_MaintenanceOrderId",
                        column: x => x.MaintenanceOrderId,
                        principalSchema: "public",
                        principalTable: "MaintenanceOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Acknowledgements_MaintenanceOrdersId",
                schema: "public",
                table: "Acknowledgements",
                column: "MaintenanceOrdersId");

            migrationBuilder.CreateIndex(
                name: "IX_VisualInstruction_MaintenanceOrderId",
                schema: "public",
                table: "VisualInstruction",
                column: "MaintenanceOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Acknowledgements",
                schema: "public");

            migrationBuilder.DropTable(
                name: "VisualInstruction",
                schema: "public");

            migrationBuilder.DropTable(
                name: "MaintenanceOrders",
                schema: "public");
        }
    }
}
