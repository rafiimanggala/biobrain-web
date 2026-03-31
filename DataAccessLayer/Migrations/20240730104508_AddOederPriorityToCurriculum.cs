using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddOederPriorityToCurriculum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderPriority",
                table: "Curricula",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE \"Curricula\" SET \"OrderPriority\"=3 WHERE \"CurriculumCode\"=1 OR \"CurriculumCode\"=2 OR \"CurriculumCode\"= 6");
            migrationBuilder.Sql("UPDATE \"Curricula\" SET \"OrderPriority\"=2 WHERE \"CurriculumCode\"=3");
            migrationBuilder.Sql("UPDATE \"Curricula\" SET \"OrderPriority\"=1 WHERE \"CurriculumCode\"=4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderPriority",
                table: "Curricula");
        }
    }
}
