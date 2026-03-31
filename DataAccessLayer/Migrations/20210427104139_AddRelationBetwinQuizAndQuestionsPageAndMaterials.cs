using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddRelationBetwinQuizAndQuestionsPageAndMaterials : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ContentTreeId",
                table: "Quizzes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ContentTreeId",
                table: "Pages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_ContentTreeId",
                table: "Quizzes",
                column: "ContentTreeId");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_ContentTreeId",
                table: "Pages",
                column: "ContentTreeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_ContentTree_ContentTreeId",
                table: "Pages",
                column: "ContentTreeId",
                principalTable: "ContentTree",
                principalColumn: "NodeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_ContentTree_ContentTreeId",
                table: "Quizzes",
                column: "ContentTreeId",
                principalTable: "ContentTree",
                principalColumn: "NodeId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pages_ContentTree_ContentTreeId",
                table: "Pages");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_ContentTree_ContentTreeId",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_ContentTreeId",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Pages_ContentTreeId",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "ContentTreeId",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "ContentTreeId",
                table: "Pages");
        }
    }
}
