using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddMarine10COurse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        INSERT INTO ""Courses""(""CourseId"", ""SubjectCode"", ""CurriculumCode"", ""Year"") VALUES('B93D42C0-A3E7-4D70-A25F-07BA8719F753', '9', 0, 10);

            ");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
