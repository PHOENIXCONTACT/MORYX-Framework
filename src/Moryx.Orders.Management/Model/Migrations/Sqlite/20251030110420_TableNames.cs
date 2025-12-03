// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moryx.Orders.Management.Model.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class TableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationAdviceEntities_OperationEntities_OperationId",
                schema: "public",
                table: "OperationAdviceEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationEntities_OrderEntities_OrderId",
                schema: "public",
                table: "OperationEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationJobReferenceEntities_OperationEntities_OperationId",
                schema: "public",
                table: "OperationJobReferenceEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationRecipeReferenceEntities_OperationEntities_OperationId",
                schema: "public",
                table: "OperationRecipeReferenceEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationReportEntities_OperationEntities_OperationId",
                schema: "public",
                table: "OperationReportEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductPartEntities_OperationEntities_OperationId",
                schema: "public",
                table: "ProductPartEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductPartEntities",
                schema: "public",
                table: "ProductPartEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderEntities",
                schema: "public",
                table: "OrderEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OperationReportEntities",
                schema: "public",
                table: "OperationReportEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OperationRecipeReferenceEntities",
                schema: "public",
                table: "OperationRecipeReferenceEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OperationJobReferenceEntities",
                schema: "public",
                table: "OperationJobReferenceEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OperationEntities",
                schema: "public",
                table: "OperationEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OperationAdviceEntities",
                schema: "public",
                table: "OperationAdviceEntities");

            migrationBuilder.RenameTable(
                name: "ProductPartEntities",
                schema: "public",
                newName: "ProductParts",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "OrderEntities",
                schema: "public",
                newName: "Orders",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "OperationReportEntities",
                schema: "public",
                newName: "OperationReports",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "OperationRecipeReferenceEntities",
                schema: "public",
                newName: "OperationRecipeReferences",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "OperationJobReferenceEntities",
                schema: "public",
                newName: "OperationJobReferences",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "OperationEntities",
                schema: "public",
                newName: "Operations",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "OperationAdviceEntities",
                schema: "public",
                newName: "OperationAdvices",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_ProductPartEntities_OperationId",
                schema: "public",
                table: "ProductParts",
                newName: "IX_ProductParts_OperationId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationReportEntities_OperationId",
                schema: "public",
                table: "OperationReports",
                newName: "IX_OperationReports_OperationId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationRecipeReferenceEntities_OperationId",
                schema: "public",
                table: "OperationRecipeReferences",
                newName: "IX_OperationRecipeReferences_OperationId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationJobReferenceEntities_OperationId",
                schema: "public",
                table: "OperationJobReferences",
                newName: "IX_OperationJobReferences_OperationId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationEntities_OrderId",
                schema: "public",
                table: "Operations",
                newName: "IX_Operations_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationAdviceEntities_OperationId",
                schema: "public",
                table: "OperationAdvices",
                newName: "IX_OperationAdvices_OperationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductParts",
                schema: "public",
                table: "ProductParts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                schema: "public",
                table: "Orders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OperationReports",
                schema: "public",
                table: "OperationReports",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OperationRecipeReferences",
                schema: "public",
                table: "OperationRecipeReferences",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OperationJobReferences",
                schema: "public",
                table: "OperationJobReferences",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Operations",
                schema: "public",
                table: "Operations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OperationAdvices",
                schema: "public",
                table: "OperationAdvices",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationAdvices_Operations_OperationId",
                schema: "public",
                table: "OperationAdvices",
                column: "OperationId",
                principalSchema: "public",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationJobReferences_Operations_OperationId",
                schema: "public",
                table: "OperationJobReferences",
                column: "OperationId",
                principalSchema: "public",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationRecipeReferences_Operations_OperationId",
                schema: "public",
                table: "OperationRecipeReferences",
                column: "OperationId",
                principalSchema: "public",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationReports_Operations_OperationId",
                schema: "public",
                table: "OperationReports",
                column: "OperationId",
                principalSchema: "public",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_Orders_OrderId",
                schema: "public",
                table: "Operations",
                column: "OrderId",
                principalSchema: "public",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductParts_Operations_OperationId",
                schema: "public",
                table: "ProductParts",
                column: "OperationId",
                principalSchema: "public",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationAdvices_Operations_OperationId",
                schema: "public",
                table: "OperationAdvices");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationJobReferences_Operations_OperationId",
                schema: "public",
                table: "OperationJobReferences");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationRecipeReferences_Operations_OperationId",
                schema: "public",
                table: "OperationRecipeReferences");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationReports_Operations_OperationId",
                schema: "public",
                table: "OperationReports");

            migrationBuilder.DropForeignKey(
                name: "FK_Operations_Orders_OrderId",
                schema: "public",
                table: "Operations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductParts_Operations_OperationId",
                schema: "public",
                table: "ProductParts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductParts",
                schema: "public",
                table: "ProductParts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                schema: "public",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Operations",
                schema: "public",
                table: "Operations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OperationReports",
                schema: "public",
                table: "OperationReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OperationRecipeReferences",
                schema: "public",
                table: "OperationRecipeReferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OperationJobReferences",
                schema: "public",
                table: "OperationJobReferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OperationAdvices",
                schema: "public",
                table: "OperationAdvices");

            migrationBuilder.RenameTable(
                name: "ProductParts",
                schema: "public",
                newName: "ProductPartEntities",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Orders",
                schema: "public",
                newName: "OrderEntities",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Operations",
                schema: "public",
                newName: "OperationEntities",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "OperationReports",
                schema: "public",
                newName: "OperationReportEntities",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "OperationRecipeReferences",
                schema: "public",
                newName: "OperationRecipeReferenceEntities",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "OperationJobReferences",
                schema: "public",
                newName: "OperationJobReferenceEntities",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "OperationAdvices",
                schema: "public",
                newName: "OperationAdviceEntities",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_ProductParts_OperationId",
                schema: "public",
                table: "ProductPartEntities",
                newName: "IX_ProductPartEntities_OperationId");

            migrationBuilder.RenameIndex(
                name: "IX_Operations_OrderId",
                schema: "public",
                table: "OperationEntities",
                newName: "IX_OperationEntities_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationReports_OperationId",
                schema: "public",
                table: "OperationReportEntities",
                newName: "IX_OperationReportEntities_OperationId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationRecipeReferences_OperationId",
                schema: "public",
                table: "OperationRecipeReferenceEntities",
                newName: "IX_OperationRecipeReferenceEntities_OperationId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationJobReferences_OperationId",
                schema: "public",
                table: "OperationJobReferenceEntities",
                newName: "IX_OperationJobReferenceEntities_OperationId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationAdvices_OperationId",
                schema: "public",
                table: "OperationAdviceEntities",
                newName: "IX_OperationAdviceEntities_OperationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductPartEntities",
                schema: "public",
                table: "ProductPartEntities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderEntities",
                schema: "public",
                table: "OrderEntities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OperationEntities",
                schema: "public",
                table: "OperationEntities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OperationReportEntities",
                schema: "public",
                table: "OperationReportEntities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OperationRecipeReferenceEntities",
                schema: "public",
                table: "OperationRecipeReferenceEntities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OperationJobReferenceEntities",
                schema: "public",
                table: "OperationJobReferenceEntities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OperationAdviceEntities",
                schema: "public",
                table: "OperationAdviceEntities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationAdviceEntities_OperationEntities_OperationId",
                schema: "public",
                table: "OperationAdviceEntities",
                column: "OperationId",
                principalSchema: "public",
                principalTable: "OperationEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationEntities_OrderEntities_OrderId",
                schema: "public",
                table: "OperationEntities",
                column: "OrderId",
                principalSchema: "public",
                principalTable: "OrderEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationJobReferenceEntities_OperationEntities_OperationId",
                schema: "public",
                table: "OperationJobReferenceEntities",
                column: "OperationId",
                principalSchema: "public",
                principalTable: "OperationEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationRecipeReferenceEntities_OperationEntities_OperationId",
                schema: "public",
                table: "OperationRecipeReferenceEntities",
                column: "OperationId",
                principalSchema: "public",
                principalTable: "OperationEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationReportEntities_OperationEntities_OperationId",
                schema: "public",
                table: "OperationReportEntities",
                column: "OperationId",
                principalSchema: "public",
                principalTable: "OperationEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPartEntities_OperationEntities_OperationId",
                schema: "public",
                table: "ProductPartEntities",
                column: "OperationId",
                principalSchema: "public",
                principalTable: "OperationEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
