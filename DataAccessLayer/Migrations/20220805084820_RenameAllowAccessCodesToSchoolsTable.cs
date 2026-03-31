using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class RenameAllowAccessCodesToSchoolsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AllowAccessCodes",
                table: "Schools",
                newName: "UseAccessCodes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UseAccessCodes",
                table: "Schools",
                newName: "AllowAccessCodes");
        }
    }
}
