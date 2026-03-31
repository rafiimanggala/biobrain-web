using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddIndexForVersionsToVersionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ContentVersion_MajorVersion_MinorVersion",
                table: "ContentVersion",
                columns: new[] { "MajorVersion", "MinorVersion" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ContentVersion_MajorVersion_MinorVersion",
                table: "ContentVersion");
        }
    }
}
