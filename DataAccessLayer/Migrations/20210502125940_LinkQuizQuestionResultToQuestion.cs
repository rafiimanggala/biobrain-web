using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class LinkQuizQuestionResultToQuestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_QuizResultQuestions_QuestionId",
                table: "QuizResultQuestions",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResultQuestions_Questions_QuestionId",
                table: "QuizResultQuestions",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizResultQuestions_Questions_QuestionId",
                table: "QuizResultQuestions");

            migrationBuilder.DropIndex(
                name: "IX_QuizResultQuestions_QuestionId",
                table: "QuizResultQuestions");
        }
    }
}
