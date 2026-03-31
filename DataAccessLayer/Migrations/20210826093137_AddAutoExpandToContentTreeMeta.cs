using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddAutoExpandToContentTreeMeta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoExpand",
                table: "ContentTreeMeta",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData("ContentTreeMeta", "Name", "Unit", "AutoExpand", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoExpand",
                table: "ContentTreeMeta");
        }
    }
}
