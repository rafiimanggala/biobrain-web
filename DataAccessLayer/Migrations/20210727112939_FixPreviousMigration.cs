using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class FixPreviousMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageMaterials_Pages_PageEntityPageId",
                table: "PageMaterials");

            migrationBuilder.DropIndex(
                name: "IX_PageMaterials_PageEntityPageId",
                table: "PageMaterials");

            migrationBuilder.DropColumn(
                name: "PageEntityPageId",
                table: "PageMaterials");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PageEntityPageId",
                table: "PageMaterials",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PageMaterials_PageEntityPageId",
                table: "PageMaterials",
                column: "PageEntityPageId");

            migrationBuilder.AddForeignKey(
                name: "FK_PageMaterials_Pages_PageEntityPageId",
                table: "PageMaterials",
                column: "PageEntityPageId",
                principalTable: "Pages",
                principalColumn: "PageId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
