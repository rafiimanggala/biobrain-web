using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class RemoveQuizName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Quizzes");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTree_CourseId",
                table: "ContentTree",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContentTree_Courses_CourseId",
                table: "ContentTree",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentTree_Courses_CourseId",
                table: "ContentTree");

            migrationBuilder.DropIndex(
                name: "IX_ContentTree_CourseId",
                table: "ContentTree");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Quizzes",
                type: "text",
                nullable: true);
        }
    }
}
