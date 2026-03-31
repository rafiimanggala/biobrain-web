using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class RenameLessonsToClassesForTeacher : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SchoolClassTeachers",
                columns: table => new
                {
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchoolClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolClassTeachers", x => new { x.TeacherId, x.SchoolClassId });
                    table.ForeignKey(
                        name: "FK_SchoolClassTeachers_SchoolClasses_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "SchoolClasses",
                        principalColumn: "SchoolClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchoolClassTeachers_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "TeacherId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchoolClassTeachers_SchoolClassId",
                table: "SchoolClassTeachers",
                column: "SchoolClassId");


            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION migrateData() RETURNS void AS $$
                DECLARE
                    data RECORD;
                BEGIN
                    FOR data IN
                        SELECT S.""TeacherId"", S.""SchoolClassId"", S.""CreatedAt"", S.""UpdatedAt""
                        FROM ""TeacherLessons"" S
                    LOOP
                        INSERT INTO ""SchoolClassTeachers"" VALUES (data.""TeacherId"", data.""SchoolClassId"", data.""CreatedAt"", data.""UpdatedAt"");
                    END LOOP;

                    RETURN;
                END;
                $$ LANGUAGE plpgsql;
            ");

            migrationBuilder.Sql(@"SELECT migrateData();");
            migrationBuilder.Sql(@"DROP FUNCTION migrateData;");


            migrationBuilder.DropTable(name: "TeacherLessons");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SchoolClassTeachers");

            migrationBuilder.CreateTable(
                name: "TeacherLessons",
                columns: table => new
                {
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchoolClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherLessons", x => new { x.TeacherId, x.SchoolClassId });
                    table.ForeignKey(
                        name: "FK_TeacherLessons_SchoolClasses_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "SchoolClasses",
                        principalColumn: "SchoolClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherLessons_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "TeacherId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLessons_SchoolClassId",
                table: "TeacherLessons",
                column: "SchoolClassId");
        }
    }
}
