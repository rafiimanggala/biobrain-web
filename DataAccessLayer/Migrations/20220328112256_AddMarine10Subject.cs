using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddMarine10Subject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
				    INSERT INTO ""Subjects""(""SubjectCode"", ""Name"") VALUES(9, 'Marine 10');
			");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
