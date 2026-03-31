using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddCouldCopyInToContentTreeMeta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CouldCopyIn",
                table: "ContentTreeMeta",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("UPDATE \"ContentTreeMeta\" SET \"CouldCopyIn\" = true WHERE \"Name\" = 'Key Knowledge'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CouldCopyIn",
                table: "ContentTreeMeta");
        }
    }
}
