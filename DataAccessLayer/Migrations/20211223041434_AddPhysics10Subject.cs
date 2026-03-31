using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddPhysics10Subject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql(@"
				    INSERT INTO ""Subjects""(""SubjectCode"", ""Name"") VALUES(8, 'Physics 10');
			");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
