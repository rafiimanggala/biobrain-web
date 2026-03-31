using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddCourseIdToContentVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql("DELETE FROM \"ContentVersion\"");
            migrationBuilder.DropIndex(
                name: "IX_ContentVersion_MajorVersion_MinorVersion",
                table: "ContentVersion");

            migrationBuilder.DropColumn(
                name: "MajorVersion",
                table: "ContentVersion");

            migrationBuilder.RenameColumn(
                name: "MinorVersion",
                table: "ContentVersion",
                newName: "Version");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "ContentVersion",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ContentVersion_CourseId",
                table: "ContentVersion",
                column: "CourseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentVersion_Version",
                table: "ContentVersion",
                column: "Version");

            migrationBuilder.AddForeignKey(
                name: "FK_ContentVersion_Courses_CourseId",
                table: "ContentVersion",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentVersion_Courses_CourseId",
                table: "ContentVersion");

            migrationBuilder.DropIndex(
                name: "IX_ContentVersion_CourseId",
                table: "ContentVersion");

            migrationBuilder.DropIndex(
                name: "IX_ContentVersion_Version",
                table: "ContentVersion");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "ContentVersion");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "ContentVersion",
                newName: "MinorVersion");

            migrationBuilder.AddColumn<long>(
                name: "MajorVersion",
                table: "ContentVersion",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ContentVersion_MajorVersion_MinorVersion",
                table: "ContentVersion",
                columns: new[] { "MajorVersion", "MinorVersion" });
        }
    }
}
