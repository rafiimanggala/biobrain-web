using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class UpdateYear10SubjectsAndCourses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql(@"
		INSERT INTO ""Subjects""(""SubjectCode"", ""Name"") VALUES (5, 'Biology 10');
	    INSERT INTO ""Subjects""(""SubjectCode"", ""Name"") VALUES(6, 'Chemistry 10');
        Update ""Subjects"" SET ""Name"" = 'Forensics 10' WHERE ""SubjectCode"" = 7;

        Update ""Courses"" SET ""SubjectCode"" = 5, ""CurriculumCode"" = 0 WHERE ""SubjectCode"" = 1 AND ""Year"" = 10;
        Update ""Courses"" SET ""SubjectCode"" = 6, ""CurriculumCode"" = 0 WHERE ""SubjectCode"" = 2 AND ""Year"" = 10;
        Update ""Courses"" SET ""CurriculumCode"" = 0 WHERE ""SubjectCode"" = 7 AND ""Year"" = 10;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
