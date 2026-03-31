using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class ChangeSingleClassPerStudentToMultiple : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "SchoolClasses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "SchoolClassStudents",
                columns: table => new
                {
                    SchoolClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolClassStudents", x => new { x.SchoolClassId, x.StudentId });
                    table.ForeignKey(
                        name: "FK_SchoolClassStudents_SchoolClasses_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "SchoolClasses",
                        principalColumn: "SchoolClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchoolClassStudents_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            //--- Migrate schoolClassId from Students to SchoolClassStudents
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION migrateData() RETURNS void AS $$
                DECLARE
                    student RECORD;
                BEGIN
                    FOR student IN
                        SELECT S.""StudentId"", S.""SchoolClassId"", S.""UpdatedAt""
                        FROM ""Students"" S
                        WHERE S.""SchoolClassId"" IS NOT NULL
                    LOOP
                        INSERT INTO ""SchoolClassStudents"" VALUES (student.""SchoolClassId"", student.""StudentId"", student.""UpdatedAt"", student.""UpdatedAt"");
                    END LOOP;

                    UPDATE ""Students""
                    SET ""SchoolClassId"" = NULL
                    WHERE 1 = 1;

                    RETURN;
                END;
                $$ LANGUAGE plpgsql;
            ");

            migrationBuilder.Sql(@"SELECT migrateData();");
            migrationBuilder.Sql(@"DROP FUNCTION migrateData;");

            //--- Migrate courseId from TeacherLessons to SchoolClasses
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION migrateData() RETURNS void AS $$
                DECLARE
                    lesson RECORD;
                BEGIN
                    FOR lesson IN
                        SELECT L.""SchoolClassId"", L.""CourseId""
                        FROM ""TeacherLessons"" L
                    LOOP
                        UPDATE ""SchoolClasses""
                        SET ""CourseId"" = lesson.""CourseId""
                        WHERE ""SchoolClassId"" = lesson.""SchoolClassId"";
                    END LOOP;

                    DELETE FROM ""SchoolClasses""
                    WHERE ""CourseId"" = '00000000-0000-0000-0000-000000000000';
                    
                    DELETE FROM ""TeacherLessons"" A
                    WHERE A.""CourseId"" <> (SELECT B.""CourseId"" FROM ""TeacherLessons"" B WHERE A.""SchoolClassId"" = B.""SchoolClassId"" AND A.""TeacherId"" = B.""TeacherId"" ORDER BY B.""CreatedAt"" LIMIT 1);

                    RETURN;
                END;
                $$ LANGUAGE plpgsql;
            ");

            migrationBuilder.Sql(@"SELECT migrateData();");
            migrationBuilder.Sql(@"DROP FUNCTION migrateData;");


            migrationBuilder.CreateIndex(
                name: "IX_SchoolClasses_CourseId",
                table: "SchoolClasses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolClassStudents_StudentId",
                table: "SchoolClassStudents",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolClasses_Courses_CourseId",
                table: "SchoolClasses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchoolClasses_Courses_CourseId",
                table: "SchoolClasses");

            migrationBuilder.DropTable(
                name: "SchoolClassStudents");

            migrationBuilder.DropIndex(
                name: "IX_SchoolClasses_CourseId",
                table: "SchoolClasses");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "SchoolClasses");
        }
    }
}
