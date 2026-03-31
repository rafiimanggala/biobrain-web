using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class RenameSubjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData("Subjects", "SubjectCode", 7, "Name", "Forensics");
            migrationBuilder.UpdateData("Subjects", "SubjectCode", 10, "Name", "Psychology");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
