using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddCourseIdToBookmarksTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "Bookmarks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Bookmarks_CourseId",
                table: "Bookmarks",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_Courses_CourseId",
                table: "Bookmarks",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_Courses_CourseId",
                table: "Bookmarks");

            migrationBuilder.DropIndex(
                name: "IX_Bookmarks_CourseId",
                table: "Bookmarks");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Bookmarks");
        }
    }
}
