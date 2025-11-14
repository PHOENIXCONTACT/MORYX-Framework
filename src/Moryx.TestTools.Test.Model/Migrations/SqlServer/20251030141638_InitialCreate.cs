using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moryx.TestTools.Test.Model.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "resources");

            migrationBuilder.CreateTable(
                name: "Cars",
                schema: "resources",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<int>(type: "int", nullable: false),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ReleaseDateLocal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReleaseDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    Performance = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Houses",
                schema: "resources",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Size = table.Column<int>(type: "int", nullable: false),
                    IsMethLabratory = table.Column<bool>(type: "bit", nullable: false),
                    IsBurnedDown = table.Column<bool>(type: "bit", nullable: false),
                    ToRent = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Houses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HugePocos",
                schema: "resources",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Float1 = table.Column<double>(type: "float", nullable: false),
                    Name1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Number1 = table.Column<int>(type: "int", nullable: false),
                    Float2 = table.Column<double>(type: "float", nullable: false),
                    Name2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Number2 = table.Column<int>(type: "int", nullable: false),
                    Float3 = table.Column<double>(type: "float", nullable: false),
                    Name3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Number3 = table.Column<int>(type: "int", nullable: false),
                    Float4 = table.Column<double>(type: "float", nullable: false),
                    Name4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Number4 = table.Column<int>(type: "int", nullable: false),
                    Float5 = table.Column<double>(type: "float", nullable: false),
                    Name5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Number5 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HugePocos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jsons",
                schema: "resources",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JsonData = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jsons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wheels",
                schema: "resources",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarId = table.Column<long>(type: "bigint", nullable: true),
                    WheelType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wheels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wheels_Cars_CarId",
                        column: x => x.CarId,
                        principalSchema: "resources",
                        principalTable: "Cars",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wheels_CarId",
                schema: "resources",
                table: "Wheels",
                column: "CarId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Houses",
                schema: "resources");

            migrationBuilder.DropTable(
                name: "HugePocos",
                schema: "resources");

            migrationBuilder.DropTable(
                name: "Jsons",
                schema: "resources");

            migrationBuilder.DropTable(
                name: "Wheels",
                schema: "resources");

            migrationBuilder.DropTable(
                name: "Cars",
                schema: "resources");
        }
    }
}
