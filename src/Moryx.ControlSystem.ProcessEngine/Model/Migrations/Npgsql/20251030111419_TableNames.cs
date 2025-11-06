using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moryx.ControlSystem.ProcessEngine.Model.Migrations.Npgsql
{
    /// <inheritdoc />
    public partial class TableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityEntities_JobEntities_JobId",
                schema: "public",
                table: "ActivityEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityEntities_ProcessEntities_ProcessId",
                schema: "public",
                table: "ActivityEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityEntities_TracingTypes_TracingTypeId",
                schema: "public",
                table: "ActivityEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_JobEntities_JobEntities_PreviousId",
                schema: "public",
                table: "JobEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_ProcessEntities_JobEntities_JobId",
                schema: "public",
                table: "ProcessEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_TokenHolderEntities_ProcessEntities_ProcessId",
                schema: "public",
                table: "TokenHolderEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TokenHolderEntities",
                schema: "public",
                table: "TokenHolderEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProcessEntities",
                schema: "public",
                table: "ProcessEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobEntities",
                schema: "public",
                table: "JobEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityEntities",
                schema: "public",
                table: "ActivityEntities");

            migrationBuilder.RenameTable(
                name: "TokenHolderEntities",
                schema: "public",
                newName: "TokenHolders",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ProcessEntities",
                schema: "public",
                newName: "Processes",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "JobEntities",
                schema: "public",
                newName: "Jobs",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ActivityEntities",
                schema: "public",
                newName: "Activities",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_TokenHolderEntities_ProcessId",
                schema: "public",
                table: "TokenHolders",
                newName: "IX_TokenHolders_ProcessId");

            migrationBuilder.RenameIndex(
                name: "IX_ProcessEntities_JobId",
                schema: "public",
                table: "Processes",
                newName: "IX_Processes_JobId");

            migrationBuilder.RenameIndex(
                name: "IX_JobEntities_PreviousId",
                schema: "public",
                table: "Jobs",
                newName: "IX_Jobs_PreviousId");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityEntities_TracingTypeId",
                schema: "public",
                table: "Activities",
                newName: "IX_Activities_TracingTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityEntities_ProcessId",
                schema: "public",
                table: "Activities",
                newName: "IX_Activities_ProcessId");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityEntities_JobId",
                schema: "public",
                table: "Activities",
                newName: "IX_Activities_JobId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "public",
                table: "Processes",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Deleted",
                schema: "public",
                table: "Processes",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                schema: "public",
                table: "Processes",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "public",
                table: "Jobs",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Deleted",
                schema: "public",
                table: "Jobs",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                schema: "public",
                table: "Jobs",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Started",
                schema: "public",
                table: "Activities",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Completed",
                schema: "public",
                table: "Activities",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TokenHolders",
                schema: "public",
                table: "TokenHolders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Processes",
                schema: "public",
                table: "Processes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Jobs",
                schema: "public",
                table: "Jobs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Activities",
                schema: "public",
                table: "Activities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Jobs_JobId",
                schema: "public",
                table: "Activities",
                column: "JobId",
                principalSchema: "public",
                principalTable: "Jobs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Processes_ProcessId",
                schema: "public",
                table: "Activities",
                column: "ProcessId",
                principalSchema: "public",
                principalTable: "Processes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_TracingTypes_TracingTypeId",
                schema: "public",
                table: "Activities",
                column: "TracingTypeId",
                principalSchema: "public",
                principalTable: "TracingTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Jobs_PreviousId",
                schema: "public",
                table: "Jobs",
                column: "PreviousId",
                principalSchema: "public",
                principalTable: "Jobs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Processes_Jobs_JobId",
                schema: "public",
                table: "Processes",
                column: "JobId",
                principalSchema: "public",
                principalTable: "Jobs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TokenHolders_Processes_ProcessId",
                schema: "public",
                table: "TokenHolders",
                column: "ProcessId",
                principalSchema: "public",
                principalTable: "Processes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Jobs_JobId",
                schema: "public",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Processes_ProcessId",
                schema: "public",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Activities_TracingTypes_TracingTypeId",
                schema: "public",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Jobs_PreviousId",
                schema: "public",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Processes_Jobs_JobId",
                schema: "public",
                table: "Processes");

            migrationBuilder.DropForeignKey(
                name: "FK_TokenHolders_Processes_ProcessId",
                schema: "public",
                table: "TokenHolders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TokenHolders",
                schema: "public",
                table: "TokenHolders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Processes",
                schema: "public",
                table: "Processes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Jobs",
                schema: "public",
                table: "Jobs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Activities",
                schema: "public",
                table: "Activities");

            migrationBuilder.RenameTable(
                name: "TokenHolders",
                schema: "public",
                newName: "TokenHolderEntities",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Processes",
                schema: "public",
                newName: "ProcessEntities",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Jobs",
                schema: "public",
                newName: "JobEntities",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Activities",
                schema: "public",
                newName: "ActivityEntities",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_TokenHolders_ProcessId",
                schema: "public",
                table: "TokenHolderEntities",
                newName: "IX_TokenHolderEntities_ProcessId");

            migrationBuilder.RenameIndex(
                name: "IX_Processes_JobId",
                schema: "public",
                table: "ProcessEntities",
                newName: "IX_ProcessEntities_JobId");

            migrationBuilder.RenameIndex(
                name: "IX_Jobs_PreviousId",
                schema: "public",
                table: "JobEntities",
                newName: "IX_JobEntities_PreviousId");

            migrationBuilder.RenameIndex(
                name: "IX_Activities_TracingTypeId",
                schema: "public",
                table: "ActivityEntities",
                newName: "IX_ActivityEntities_TracingTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Activities_ProcessId",
                schema: "public",
                table: "ActivityEntities",
                newName: "IX_ActivityEntities_ProcessId");

            migrationBuilder.RenameIndex(
                name: "IX_Activities_JobId",
                schema: "public",
                table: "ActivityEntities",
                newName: "IX_ActivityEntities_JobId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "public",
                table: "ProcessEntities",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Deleted",
                schema: "public",
                table: "ProcessEntities",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                schema: "public",
                table: "ProcessEntities",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "public",
                table: "JobEntities",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Deleted",
                schema: "public",
                table: "JobEntities",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                schema: "public",
                table: "JobEntities",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Started",
                schema: "public",
                table: "ActivityEntities",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Completed",
                schema: "public",
                table: "ActivityEntities",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TokenHolderEntities",
                schema: "public",
                table: "TokenHolderEntities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProcessEntities",
                schema: "public",
                table: "ProcessEntities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobEntities",
                schema: "public",
                table: "JobEntities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityEntities",
                schema: "public",
                table: "ActivityEntities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityEntities_JobEntities_JobId",
                schema: "public",
                table: "ActivityEntities",
                column: "JobId",
                principalSchema: "public",
                principalTable: "JobEntities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityEntities_ProcessEntities_ProcessId",
                schema: "public",
                table: "ActivityEntities",
                column: "ProcessId",
                principalSchema: "public",
                principalTable: "ProcessEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityEntities_TracingTypes_TracingTypeId",
                schema: "public",
                table: "ActivityEntities",
                column: "TracingTypeId",
                principalSchema: "public",
                principalTable: "TracingTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobEntities_JobEntities_PreviousId",
                schema: "public",
                table: "JobEntities",
                column: "PreviousId",
                principalSchema: "public",
                principalTable: "JobEntities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessEntities_JobEntities_JobId",
                schema: "public",
                table: "ProcessEntities",
                column: "JobId",
                principalSchema: "public",
                principalTable: "JobEntities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TokenHolderEntities_ProcessEntities_ProcessId",
                schema: "public",
                table: "TokenHolderEntities",
                column: "ProcessId",
                principalSchema: "public",
                principalTable: "ProcessEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
