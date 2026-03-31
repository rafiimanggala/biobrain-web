using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DataAccessLayer.Migrations
{
    public partial class AddCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Curricula",
                columns: table => new
                {
                    CurriculumCode = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Curricula", x => x.CurriculumCode);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    SubjectCode = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.SubjectCode);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectCode = table.Column<int>(type: "integer", nullable: false),
                    CurriculumCode = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.CourseId);
                    table.ForeignKey(
                        name: "FK_Courses_Curricula_CurriculumCode",
                        column: x => x.CurriculumCode,
                        principalTable: "Curricula",
                        principalColumn: "CurriculumCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Courses_Subjects_SubjectCode",
                        column: x => x.SubjectCode,
                        principalTable: "Subjects",
                        principalColumn: "SubjectCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CurriculumCode",
                table: "Courses",
                column: "CurriculumCode");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_SubjectCode_CurriculumCode",
                table: "Courses",
                columns: new[] { "SubjectCode", "CurriculumCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Curricula_Name",
                table: "Curricula",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_Name",
                table: "Subjects",
                column: "Name",
                unique: true);


            migrationBuilder.InsertData("Curricula", new[] {"CurriculumCode", "Name"}, new object[,]
                                                                                       {
                                                                                           {1, "VCE"}
                                                                                       });

            migrationBuilder.InsertData("Subjects", new[] { "SubjectCode", "Name"}, new object[,]
                                                                                    {
                                                                                        {1, "Biology"},
                                                                                        {2, "Chemistry"},
                                                                                        {3, "Physics"},
                                                                                    });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Curricula");

            migrationBuilder.DropTable(
                name: "Subjects");
        }
    }
}
