using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddIsGenericToCurriculum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsGeneric",
                table: "Curricula",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData("Curricula", "CurriculumCode", 0, "IsGeneric", true);
            migrationBuilder.UpdateData("Curricula", "CurriculumCode", 0, "Name", "IB");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGeneric",
                table: "Curricula");
        }
    }
}
