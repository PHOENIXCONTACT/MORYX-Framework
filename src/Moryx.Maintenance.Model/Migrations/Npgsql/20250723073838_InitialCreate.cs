using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Moryx.Maintenance.Management.Model.Migrations.Npgsql
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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ResourceId = table.Column<long>(type: "bigint", nullable: false),
                    IntervalType = table.Column<string>(type: "text", nullable: true),
                    IntervalData = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Block = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OperatorId = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    MaintenanceOrdersId = table.Column<long>(type: "bigint", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Preview = table.Column<string>(type: "text", nullable: true),
                    MaintenanceOrderId = table.Column<long>(type: "bigint", nullable: false)
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
