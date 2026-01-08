// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moryx.Operators.Management.Model.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class TableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResourceLinkEntities_OperatorEntities_OperatorEntityId",
                schema: "public",
                table: "ResourceLinkEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillEntities_SkillTypeEntities_SkillTypeId",
                schema: "public",
                table: "SkillEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkillTypeEntities",
                schema: "public",
                table: "SkillTypeEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkillEntities",
                schema: "public",
                table: "SkillEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResourceLinkEntities",
                schema: "public",
                table: "ResourceLinkEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OperatorEntities",
                schema: "public",
                table: "OperatorEntities");

            migrationBuilder.RenameTable(
                name: "SkillTypeEntities",
                schema: "public",
                newName: "SkillTypes",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "SkillEntities",
                schema: "public",
                newName: "Skills",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ResourceLinkEntities",
                schema: "public",
                newName: "ResourceLinks",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "OperatorEntities",
                schema: "public",
                newName: "Operators",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_SkillEntities_SkillTypeId",
                schema: "public",
                table: "Skills",
                newName: "IX_Skills_SkillTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceLinkEntities_OperatorEntityId",
                schema: "public",
                table: "ResourceLinks",
                newName: "IX_ResourceLinks_OperatorEntityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkillTypes",
                schema: "public",
                table: "SkillTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Skills",
                schema: "public",
                table: "Skills",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResourceLinks",
                schema: "public",
                table: "ResourceLinks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Operators",
                schema: "public",
                table: "Operators",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceLinks_Operators_OperatorEntityId",
                schema: "public",
                table: "ResourceLinks",
                column: "OperatorEntityId",
                principalSchema: "public",
                principalTable: "Operators",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_SkillTypes_SkillTypeId",
                schema: "public",
                table: "Skills",
                column: "SkillTypeId",
                principalSchema: "public",
                principalTable: "SkillTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResourceLinks_Operators_OperatorEntityId",
                schema: "public",
                table: "ResourceLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_SkillTypes_SkillTypeId",
                schema: "public",
                table: "Skills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkillTypes",
                schema: "public",
                table: "SkillTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Skills",
                schema: "public",
                table: "Skills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResourceLinks",
                schema: "public",
                table: "ResourceLinks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Operators",
                schema: "public",
                table: "Operators");

            migrationBuilder.RenameTable(
                name: "SkillTypes",
                schema: "public",
                newName: "SkillTypeEntities",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Skills",
                schema: "public",
                newName: "SkillEntities",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ResourceLinks",
                schema: "public",
                newName: "ResourceLinkEntities",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Operators",
                schema: "public",
                newName: "OperatorEntities",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_Skills_SkillTypeId",
                schema: "public",
                table: "SkillEntities",
                newName: "IX_SkillEntities_SkillTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceLinks_OperatorEntityId",
                schema: "public",
                table: "ResourceLinkEntities",
                newName: "IX_ResourceLinkEntities_OperatorEntityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkillTypeEntities",
                schema: "public",
                table: "SkillTypeEntities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkillEntities",
                schema: "public",
                table: "SkillEntities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResourceLinkEntities",
                schema: "public",
                table: "ResourceLinkEntities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OperatorEntities",
                schema: "public",
                table: "OperatorEntities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceLinkEntities_OperatorEntities_OperatorEntityId",
                schema: "public",
                table: "ResourceLinkEntities",
                column: "OperatorEntityId",
                principalSchema: "public",
                principalTable: "OperatorEntities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SkillEntities_SkillTypeEntities_SkillTypeId",
                schema: "public",
                table: "SkillEntities",
                column: "SkillTypeId",
                principalSchema: "public",
                principalTable: "SkillTypeEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
