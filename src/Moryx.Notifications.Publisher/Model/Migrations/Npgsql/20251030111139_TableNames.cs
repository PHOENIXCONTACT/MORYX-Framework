using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moryx.Notifications.Publisher.Model.Migrations.Npgsql
{
    /// <inheritdoc />
    public partial class TableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationEntities_NotificationTypeEntities_TypeId",
                schema: "public",
                table: "NotificationEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationTypeEntities",
                schema: "public",
                table: "NotificationTypeEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationEntities",
                schema: "public",
                table: "NotificationEntities");

            migrationBuilder.RenameTable(
                name: "NotificationTypeEntities",
                schema: "public",
                newName: "NotificationTypes",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "NotificationEntities",
                schema: "public",
                newName: "Notifications",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationEntities_TypeId",
                schema: "public",
                table: "Notifications",
                newName: "IX_Notifications_TypeId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                schema: "public",
                table: "Notifications",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Acknowledged",
                schema: "public",
                table: "Notifications",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationTypes",
                schema: "public",
                table: "NotificationTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                schema: "public",
                table: "Notifications",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NotificationTypes_TypeId",
                schema: "public",
                table: "Notifications",
                column: "TypeId",
                principalSchema: "public",
                principalTable: "NotificationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NotificationTypes_TypeId",
                schema: "public",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationTypes",
                schema: "public",
                table: "NotificationTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                schema: "public",
                table: "Notifications");

            migrationBuilder.RenameTable(
                name: "NotificationTypes",
                schema: "public",
                newName: "NotificationTypeEntities",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Notifications",
                schema: "public",
                newName: "NotificationEntities",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_TypeId",
                schema: "public",
                table: "NotificationEntities",
                newName: "IX_NotificationEntities_TypeId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                schema: "public",
                table: "NotificationEntities",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Acknowledged",
                schema: "public",
                table: "NotificationEntities",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationTypeEntities",
                schema: "public",
                table: "NotificationTypeEntities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationEntities",
                schema: "public",
                table: "NotificationEntities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationEntities_NotificationTypeEntities_TypeId",
                schema: "public",
                table: "NotificationEntities",
                column: "TypeId",
                principalSchema: "public",
                principalTable: "NotificationTypeEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
