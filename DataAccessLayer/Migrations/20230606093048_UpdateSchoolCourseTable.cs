using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class UpdateSchoolCourseTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchoolCourseEntity_Courses_CourseId",
                table: "SchoolCourseEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolCourseEntity_Schools_SchoolId",
                table: "SchoolCourseEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SchoolCourseEntity",
                table: "SchoolCourseEntity");

            migrationBuilder.RenameTable(
                name: "SchoolCourseEntity",
                newName: "SchoolCourses");

            migrationBuilder.RenameIndex(
                name: "IX_SchoolCourseEntity_SchoolId",
                table: "SchoolCourses",
                newName: "IX_SchoolCourses_SchoolId");

            migrationBuilder.RenameIndex(
                name: "IX_SchoolCourseEntity_CourseId",
                table: "SchoolCourses",
                newName: "IX_SchoolCourses_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SchoolCourses",
                table: "SchoolCourses",
                column: "SchoolCourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolCourses_Courses_CourseId",
                table: "SchoolCourses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolCourses_Schools_SchoolId",
                table: "SchoolCourses",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "SchoolId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchoolCourses_Courses_CourseId",
                table: "SchoolCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolCourses_Schools_SchoolId",
                table: "SchoolCourses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SchoolCourses",
                table: "SchoolCourses");

            migrationBuilder.RenameTable(
                name: "SchoolCourses",
                newName: "SchoolCourseEntity");

            migrationBuilder.RenameIndex(
                name: "IX_SchoolCourses_SchoolId",
                table: "SchoolCourseEntity",
                newName: "IX_SchoolCourseEntity_SchoolId");

            migrationBuilder.RenameIndex(
                name: "IX_SchoolCourses_CourseId",
                table: "SchoolCourseEntity",
                newName: "IX_SchoolCourseEntity_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SchoolCourseEntity",
                table: "SchoolCourseEntity",
                column: "SchoolCourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolCourseEntity_Courses_CourseId",
                table: "SchoolCourseEntity",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolCourseEntity_Schools_SchoolId",
                table: "SchoolCourseEntity",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "SchoolId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
