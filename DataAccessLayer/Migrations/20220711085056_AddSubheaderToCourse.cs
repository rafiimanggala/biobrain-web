using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddSubheaderToCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubHeader",
                table: "Courses",
                type: "text",
                nullable: true);

            // SACE - Chemistry - 11
            migrationBuilder.UpdateData("Courses", "CourseId", "7AE0382C-D9A2-4CC9-A894-C6A30FDC7B4B", "SubHeader", "Stage 1");
            // SACE - Biology - 11
            migrationBuilder.UpdateData("Courses", "CourseId", "2144DE6D-175A-451D-A59C-F6B888DE1C61", "SubHeader", "Stage 1");
            // SACE - Chemistry - 12
            migrationBuilder.UpdateData("Courses", "CourseId", "8A641DA7-FE67-49B7-8C5F-BA3E43D7AAEC", "SubHeader", "Stage 2");
            // SACE - Biology - 12
            migrationBuilder.UpdateData("Courses", "CourseId", "9A9784E4-C602-41C3-9C19-B02C28A4AB5E", "SubHeader", "Stage 2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubHeader",
                table: "Courses");
        }
    }
}
