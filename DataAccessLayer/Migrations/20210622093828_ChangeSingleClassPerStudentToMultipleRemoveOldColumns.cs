using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class ChangeSingleClassPerStudentToMultipleRemoveOldColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_SchoolClasses_SchoolClassId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherLessons_Courses_CourseId",
                table: "TeacherLessons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeacherLessons",
                table: "TeacherLessons");

            migrationBuilder.DropIndex(
                name: "IX_TeacherLessons_CourseId",
                table: "TeacherLessons");

            migrationBuilder.DropIndex(
                name: "IX_TeacherLessons_TeacherId_CourseId_SchoolClassId",
                table: "TeacherLessons");

            migrationBuilder.DropIndex(
                name: "IX_Students_SchoolClassId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "TeacherLessonId",
                table: "TeacherLessons");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "TeacherLessons");

            migrationBuilder.DropColumn(
                name: "SchoolClassId",
                table: "Students");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeacherLessons",
                table: "TeacherLessons",
                columns: new[] { "TeacherId", "SchoolClassId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TeacherLessons",
                table: "TeacherLessons");

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherLessonId",
                table: "TeacherLessons",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "TeacherLessons",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SchoolClassId",
                table: "Students",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeacherLessons",
                table: "TeacherLessons",
                column: "TeacherLessonId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLessons_CourseId",
                table: "TeacherLessons",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLessons_TeacherId_CourseId_SchoolClassId",
                table: "TeacherLessons",
                columns: new[] { "TeacherId", "CourseId", "SchoolClassId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_SchoolClassId",
                table: "Students",
                column: "SchoolClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_SchoolClasses_SchoolClassId",
                table: "Students",
                column: "SchoolClassId",
                principalTable: "SchoolClasses",
                principalColumn: "SchoolClassId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherLessons_Courses_CourseId",
                table: "TeacherLessons",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
