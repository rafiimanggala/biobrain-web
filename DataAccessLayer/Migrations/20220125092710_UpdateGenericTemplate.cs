using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class UpdateGenericTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql("UPDATE \"Templates\" SET \"Value\" = '<div>{0} </div><div>{1} </div><div>Level {2:i} </div>' WHERE \"TemplateId\" = 'D19D9A60-3F5E-4CB8-8D07-B165AE6D1CD0'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
