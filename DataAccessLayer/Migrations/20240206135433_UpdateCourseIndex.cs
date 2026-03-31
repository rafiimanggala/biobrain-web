using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class UpdateCourseIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Courses_SubjectCode_CurriculumCode_Year",
                table: "Courses");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_SubjectCode_CurriculumCode_Year",
                table: "Courses",
                columns: new[] { "SubjectCode", "CurriculumCode", "Year" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Courses_SubjectCode_CurriculumCode_Year",
                table: "Courses");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_SubjectCode_CurriculumCode_Year",
                table: "Courses",
                columns: new[] { "SubjectCode", "CurriculumCode", "Year" },
                unique: true);
        }
    }
}
