using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddQuizAssignmentToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizStudentAssignments_Students_AssignedToStudentId",
                table: "QuizStudentAssignments");

            migrationBuilder.RenameColumn(
                name: "AssignedToStudentId",
                table: "QuizStudentAssignments",
                newName: "AssignedToUserId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizStudentAssignments_AssignedToStudentId",
                table: "QuizStudentAssignments",
                newName: "IX_QuizStudentAssignments_AssignedToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizStudentAssignments_AspNetUsers_AssignedToUserId",
                table: "QuizStudentAssignments",
                column: "AssignedToUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizStudentAssignments_AspNetUsers_AssignedToUserId",
                table: "QuizStudentAssignments");

            migrationBuilder.RenameColumn(
                name: "AssignedToUserId",
                table: "QuizStudentAssignments",
                newName: "AssignedToStudentId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizStudentAssignments_AssignedToUserId",
                table: "QuizStudentAssignments",
                newName: "IX_QuizStudentAssignments_AssignedToStudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizStudentAssignments_Students_AssignedToStudentId",
                table: "QuizStudentAssignments",
                column: "AssignedToStudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
