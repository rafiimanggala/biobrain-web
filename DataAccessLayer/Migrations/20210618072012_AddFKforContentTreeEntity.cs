using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddFKforContentTreeEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ContentTree_ParentId",
                table: "ContentTree",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContentTree_ContentTree_ParentId",
                table: "ContentTree",
                column: "ParentId",
                principalTable: "ContentTree",
                principalColumn: "NodeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentTree_ContentTree_ParentId",
                table: "ContentTree");

            migrationBuilder.DropIndex(
                name: "IX_ContentTree_ParentId",
                table: "ContentTree");
        }
    }
}
