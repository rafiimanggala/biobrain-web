using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddQuizzes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    QuizId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.QuizId);
                });

            migrationBuilder.CreateTable(
                name: "QuizAssignments",
                columns: table => new
                {
                    QuizAssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuizId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedByTeacherId = table.Column<Guid>(type: "uuid", nullable: true),
                    SchoolClassId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAssignments", x => x.QuizAssignmentId);
                    table.ForeignKey(
                        name: "FK_QuizAssignments_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "QuizId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizAssignments_SchoolClasses_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "SchoolClasses",
                        principalColumn: "SchoolClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizAssignments_Teachers_AssignedByTeacherId",
                        column: x => x.AssignedByTeacherId,
                        principalTable: "Teachers",
                        principalColumn: "TeacherId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "QuizStudentAssignmentEntity",
                columns: table => new
                {
                    QuizStudentAssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuizAssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedToStudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttemptNumber = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizStudentAssignmentEntity", x => x.QuizStudentAssignmentId);
                    table.ForeignKey(
                        name: "FK_QuizStudentAssignmentEntity_QuizAssignments_QuizAssignmentId",
                        column: x => x.QuizAssignmentId,
                        principalTable: "QuizAssignments",
                        principalColumn: "QuizAssignmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizStudentAssignmentEntity_Students_AssignedToStudentId",
                        column: x => x.AssignedToStudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizResults",
                columns: table => new
                {
                    QuizStudentAssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<double>(type: "double precision", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    StaredAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizResults", x => x.QuizStudentAssignmentId);
                    table.ForeignKey(
                        name: "FK_QuizResults_QuizStudentAssignmentEntity_QuizStudentAssignme~",
                        column: x => x.QuizStudentAssignmentId,
                        principalTable: "QuizStudentAssignmentEntity",
                        principalColumn: "QuizStudentAssignmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizResultQuestions",
                columns: table => new
                {
                    QuizResultId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizResultQuestions", x => new { x.QuizResultId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_QuizResultQuestions_QuizResults_QuizResultId",
                        column: x => x.QuizResultId,
                        principalTable: "QuizResults",
                        principalColumn: "QuizStudentAssignmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizAssignments_AssignedByTeacherId",
                table: "QuizAssignments",
                column: "AssignedByTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAssignments_QuizId",
                table: "QuizAssignments",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAssignments_SchoolClassId",
                table: "QuizAssignments",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizStudentAssignmentEntity_AssignedToStudentId",
                table: "QuizStudentAssignmentEntity",
                column: "AssignedToStudentId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizStudentAssignmentEntity_QuizAssignmentId",
                table: "QuizStudentAssignmentEntity",
                column: "QuizAssignmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizResultQuestions");

            migrationBuilder.DropTable(
                name: "QuizResults");

            migrationBuilder.DropTable(
                name: "QuizStudentAssignmentEntity");

            migrationBuilder.DropTable(
                name: "QuizAssignments");

            migrationBuilder.DropTable(
                name: "Quizzes");
        }
    }
}
