using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddLearningMaterialAssignments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAtLocal",
                table: "QuizStudentAssignments",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAtUtc",
                table: "QuizStudentAssignments",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DueAtLocal",
                table: "QuizStudentAssignments",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DueAtUtc",
                table: "QuizStudentAssignments",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAtLocal",
                table: "QuizAssignments",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAtUtc",
                table: "QuizAssignments",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DueAtLocal",
                table: "QuizAssignments",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DueAtUtc",
                table: "QuizAssignments",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LearningMaterialAssignments",
                columns: table => new
                {
                    LearningMaterialAssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentTreeNodeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    SchoolClassId = table.Column<Guid>(type: "uuid", nullable: true),
                    DueAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DueAtLocal = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AssignedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AssignedAtLocal = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningMaterialAssignments", x => x.LearningMaterialAssignmentId);
                    table.ForeignKey(
                        name: "FK_LearningMaterialAssignments_AspNetUsers_AssignedByUserId",
                        column: x => x.AssignedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LearningMaterialAssignments_ContentTree_ContentTreeNodeId",
                        column: x => x.ContentTreeNodeId,
                        principalTable: "ContentTree",
                        principalColumn: "NodeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LearningMaterialAssignments_SchoolClasses_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "SchoolClasses",
                        principalColumn: "SchoolClassId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LearningMaterialUserAssignments",
                columns: table => new
                {
                    LearningMaterialUserAssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    LearningMaterialAssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedToUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DueAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DueAtLocal = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AssignedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AssignedAtLocal = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningMaterialUserAssignments", x => x.LearningMaterialUserAssignmentId);
                    table.ForeignKey(
                        name: "FK_LearningMaterialUserAssignments_AspNetUsers_AssignedToUserId",
                        column: x => x.AssignedToUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LearningMaterialUserAssignments_LearningMaterialAssignments~",
                        column: x => x.LearningMaterialAssignmentId,
                        principalTable: "LearningMaterialAssignments",
                        principalColumn: "LearningMaterialAssignmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LearningMaterialAssignments_AssignedByUserId",
                table: "LearningMaterialAssignments",
                column: "AssignedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningMaterialAssignments_ContentTreeNodeId",
                table: "LearningMaterialAssignments",
                column: "ContentTreeNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningMaterialAssignments_SchoolClassId",
                table: "LearningMaterialAssignments",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningMaterialUserAssignments_AssignedToUserId",
                table: "LearningMaterialUserAssignments",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningMaterialUserAssignments_LearningMaterialAssignmentId",
                table: "LearningMaterialUserAssignments",
                column: "LearningMaterialAssignmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LearningMaterialUserAssignments");

            migrationBuilder.DropTable(
                name: "LearningMaterialAssignments");

            migrationBuilder.DropColumn(
                name: "AssignedAtLocal",
                table: "QuizStudentAssignments");

            migrationBuilder.DropColumn(
                name: "AssignedAtUtc",
                table: "QuizStudentAssignments");

            migrationBuilder.DropColumn(
                name: "DueAtLocal",
                table: "QuizStudentAssignments");

            migrationBuilder.DropColumn(
                name: "DueAtUtc",
                table: "QuizStudentAssignments");

            migrationBuilder.DropColumn(
                name: "AssignedAtLocal",
                table: "QuizAssignments");

            migrationBuilder.DropColumn(
                name: "AssignedAtUtc",
                table: "QuizAssignments");

            migrationBuilder.DropColumn(
                name: "DueAtLocal",
                table: "QuizAssignments");

            migrationBuilder.DropColumn(
                name: "DueAtUtc",
                table: "QuizAssignments");
        }
    }
}
