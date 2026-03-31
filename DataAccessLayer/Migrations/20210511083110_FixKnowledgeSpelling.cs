using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class FixKnowledgeSpelling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql("UPDATE \"ContentTreeMeta\" SET \"Name\" = 'Key Knowledge' WHERE \"Name\" LIKE 'Key Knoledge'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
