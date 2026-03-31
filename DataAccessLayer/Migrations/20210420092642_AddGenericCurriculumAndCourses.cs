using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddGenericCurriculumAndCourses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.InsertData("Curricula", new[] { "CurriculumCode", "Name" }, new object[,]
	        {
		        {0, "Generic"}
	        });

	        migrationBuilder.InsertData("Courses", new[] { "CourseId", "SubjectCode", "CurriculumCode" }, new object[,]
	        {
		        {Guid.Parse("16D4754F-4219-4271-B6CE-EE563EE9C0A5"), 1, 0},
		        {Guid.Parse("AA91B5FB-B93D-489F-901B-63C4519D63E0"), 2, 0},
		        {Guid.Parse("8E698E20-7961-4A81-A62F-E8255F6CFA60"), 3, 0},
	        });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
