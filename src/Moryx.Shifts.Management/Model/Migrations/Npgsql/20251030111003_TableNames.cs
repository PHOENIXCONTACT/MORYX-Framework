// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moryx.Shifts.Management.Model.Migrations.Npgsql
{
    /// <inheritdoc />
    public partial class TableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShiftAssignementEntities_ShiftEntities_ShiftId",
                schema: "public",
                table: "ShiftAssignementEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftEntities_ShiftTypeEntities_ShiftTypeId",
                schema: "public",
                table: "ShiftEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShiftTypeEntities",
                schema: "public",
                table: "ShiftTypeEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShiftEntities",
                schema: "public",
                table: "ShiftEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShiftAssignementEntities",
                schema: "public",
                table: "ShiftAssignementEntities");

            migrationBuilder.RenameTable(
                name: "ShiftTypeEntities",
                schema: "public",
                newName: "ShiftTypes",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ShiftEntities",
                schema: "public",
                newName: "Shifts",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ShiftAssignementEntities",
                schema: "public",
                newName: "ShiftAssignements",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_ShiftEntities_ShiftTypeId",
                schema: "public",
                table: "Shifts",
                newName: "IX_Shifts_ShiftTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ShiftAssignementEntities_ShiftId",
                schema: "public",
                table: "ShiftAssignements",
                newName: "IX_ShiftAssignements_ShiftId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShiftTypes",
                schema: "public",
                table: "ShiftTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shifts",
                schema: "public",
                table: "Shifts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShiftAssignements",
                schema: "public",
                table: "ShiftAssignements",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftAssignements_Shifts_ShiftId",
                schema: "public",
                table: "ShiftAssignements",
                column: "ShiftId",
                principalSchema: "public",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_ShiftTypes_ShiftTypeId",
                schema: "public",
                table: "Shifts",
                column: "ShiftTypeId",
                principalSchema: "public",
                principalTable: "ShiftTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShiftAssignements_Shifts_ShiftId",
                schema: "public",
                table: "ShiftAssignements");

            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_ShiftTypes_ShiftTypeId",
                schema: "public",
                table: "Shifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShiftTypes",
                schema: "public",
                table: "ShiftTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shifts",
                schema: "public",
                table: "Shifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShiftAssignements",
                schema: "public",
                table: "ShiftAssignements");

            migrationBuilder.RenameTable(
                name: "ShiftTypes",
                schema: "public",
                newName: "ShiftTypeEntities",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Shifts",
                schema: "public",
                newName: "ShiftEntities",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ShiftAssignements",
                schema: "public",
                newName: "ShiftAssignementEntities",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_Shifts_ShiftTypeId",
                schema: "public",
                table: "ShiftEntities",
                newName: "IX_ShiftEntities_ShiftTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ShiftAssignements_ShiftId",
                schema: "public",
                table: "ShiftAssignementEntities",
                newName: "IX_ShiftAssignementEntities_ShiftId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShiftTypeEntities",
                schema: "public",
                table: "ShiftTypeEntities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShiftEntities",
                schema: "public",
                table: "ShiftEntities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShiftAssignementEntities",
                schema: "public",
                table: "ShiftAssignementEntities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftAssignementEntities_ShiftEntities_ShiftId",
                schema: "public",
                table: "ShiftAssignementEntities",
                column: "ShiftId",
                principalSchema: "public",
                principalTable: "ShiftEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftEntities_ShiftTypeEntities_ShiftTypeId",
                schema: "public",
                table: "ShiftEntities",
                column: "ShiftTypeId",
                principalSchema: "public",
                principalTable: "ShiftTypeEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
