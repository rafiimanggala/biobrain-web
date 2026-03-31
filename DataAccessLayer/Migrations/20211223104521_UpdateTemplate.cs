using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class UpdateTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql("UPDATE \"Templates\" SET \"Value\" = '<div>Unit {0:i} AOS {1:i} </div><div>{3}</div><div> Level {4:i}</div>' WHERE \"TemplateId\" = '1BC16A3B-FA9D-4EE9-B6FF-FB0B5E886A57'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
