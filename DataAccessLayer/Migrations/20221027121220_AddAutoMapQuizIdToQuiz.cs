using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddAutoMapQuizIdToQuiz : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AutoMapQuizId",
                table: "Quizzes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_AutoMapQuizId",
                table: "Quizzes",
                column: "AutoMapQuizId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Quizzes_AutoMapQuizId",
                table: "Quizzes",
                column: "AutoMapQuizId",
                principalTable: "Quizzes",
                principalColumn: "QuizId",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Quizzes_AutoMapQuizId",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_AutoMapQuizId",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "AutoMapQuizId",
                table: "Quizzes");
        }
    }
}
