using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class RenameTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizResults_QuizStudentAssignmentEntity_QuizStudentAssignme~",
                table: "QuizResults");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizStudentAssignmentEntity_QuizAssignments_QuizAssignmentId",
                table: "QuizStudentAssignmentEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizStudentAssignmentEntity_Students_AssignedToStudentId",
                table: "QuizStudentAssignmentEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuizStudentAssignmentEntity",
                table: "QuizStudentAssignmentEntity");

            migrationBuilder.RenameTable(
                name: "QuizStudentAssignmentEntity",
                newName: "QuizStudentAssignments");

            migrationBuilder.RenameIndex(
                name: "IX_QuizStudentAssignmentEntity_QuizAssignmentId",
                table: "QuizStudentAssignments",
                newName: "IX_QuizStudentAssignments_QuizAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizStudentAssignmentEntity_AssignedToStudentId",
                table: "QuizStudentAssignments",
                newName: "IX_QuizStudentAssignments_AssignedToStudentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuizStudentAssignments",
                table: "QuizStudentAssignments",
                column: "QuizStudentAssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResults_QuizStudentAssignments_QuizStudentAssignmentId",
                table: "QuizResults",
                column: "QuizStudentAssignmentId",
                principalTable: "QuizStudentAssignments",
                principalColumn: "QuizStudentAssignmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizStudentAssignments_QuizAssignments_QuizAssignmentId",
                table: "QuizStudentAssignments",
                column: "QuizAssignmentId",
                principalTable: "QuizAssignments",
                principalColumn: "QuizAssignmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizStudentAssignments_Students_AssignedToStudentId",
                table: "QuizStudentAssignments",
                column: "AssignedToStudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizResults_QuizStudentAssignments_QuizStudentAssignmentId",
                table: "QuizResults");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizStudentAssignments_QuizAssignments_QuizAssignmentId",
                table: "QuizStudentAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizStudentAssignments_Students_AssignedToStudentId",
                table: "QuizStudentAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuizStudentAssignments",
                table: "QuizStudentAssignments");

            migrationBuilder.RenameTable(
                name: "QuizStudentAssignments",
                newName: "QuizStudentAssignmentEntity");

            migrationBuilder.RenameIndex(
                name: "IX_QuizStudentAssignments_QuizAssignmentId",
                table: "QuizStudentAssignmentEntity",
                newName: "IX_QuizStudentAssignmentEntity_QuizAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizStudentAssignments_AssignedToStudentId",
                table: "QuizStudentAssignmentEntity",
                newName: "IX_QuizStudentAssignmentEntity_AssignedToStudentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuizStudentAssignmentEntity",
                table: "QuizStudentAssignmentEntity",
                column: "QuizStudentAssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResults_QuizStudentAssignmentEntity_QuizStudentAssignme~",
                table: "QuizResults",
                column: "QuizStudentAssignmentId",
                principalTable: "QuizStudentAssignmentEntity",
                principalColumn: "QuizStudentAssignmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizStudentAssignmentEntity_QuizAssignments_QuizAssignmentId",
                table: "QuizStudentAssignmentEntity",
                column: "QuizAssignmentId",
                principalTable: "QuizAssignments",
                principalColumn: "QuizAssignmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizStudentAssignmentEntity_Students_AssignedToStudentId",
                table: "QuizStudentAssignmentEntity",
                column: "AssignedToStudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
