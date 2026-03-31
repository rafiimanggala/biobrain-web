using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddStudentCurriculumSetsTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentCurriculumSets",
                columns: table => new
                {
                    StudentCurriculumSetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    MainCurriculumCode = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCurriculumSets", x => x.StudentCurriculumSetId);
                    table.ForeignKey(
                        name: "FK_StudentCurriculumSets_Curricula_MainCurriculumCode",
                        column: x => x.MainCurriculumCode,
                        principalTable: "Curricula",
                        principalColumn: "CurriculumCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentCurriculumSetCountries",
                columns: table => new
                {
                    StudentCurriculumSetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    States = table.Column<string[]>(type: "text[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCurriculumSetCountries", x => new { x.StudentCurriculumSetId, x.Country });
                    table.ForeignKey(
                        name: "FK_StudentCurriculumSetCountries_StudentCurriculumSets_Student~",
                        column: x => x.StudentCurriculumSetId,
                        principalTable: "StudentCurriculumSets",
                        principalColumn: "StudentCurriculumSetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentCurriculumSetEntries",
                columns: table => new
                {
                    StudentCurriculumSetId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCurriculumSetEntries", x => new { x.StudentCurriculumSetId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_StudentCurriculumSetEntries_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentCurriculumSetEntries_StudentCurriculumSets_StudentCu~",
                        column: x => x.StudentCurriculumSetId,
                        principalTable: "StudentCurriculumSets",
                        principalColumn: "StudentCurriculumSetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentCurriculumSetEntries_CourseId",
                table: "StudentCurriculumSetEntries",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCurriculumSets_MainCurriculumCode",
                table: "StudentCurriculumSets",
                column: "MainCurriculumCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentCurriculumSetCountries");

            migrationBuilder.DropTable(
                name: "StudentCurriculumSetEntries");

            migrationBuilder.DropTable(
                name: "StudentCurriculumSets");
        }
    }
}
