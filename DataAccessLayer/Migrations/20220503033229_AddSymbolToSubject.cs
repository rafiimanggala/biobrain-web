using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddSymbolToSubject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Symbol",
                table: "Subjects",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData("Subjects", "SubjectCode", 1, "Symbol", "🧬");
            migrationBuilder.UpdateData("Subjects", "SubjectCode", 2, "Symbol", "🧪");
            migrationBuilder.UpdateData("Subjects", "SubjectCode", 3, "Symbol", "💡");
            migrationBuilder.UpdateData("Subjects", "SubjectCode", 5, "Symbol", "🧬");
            migrationBuilder.UpdateData("Subjects", "SubjectCode", 6, "Symbol", "🧪");
            migrationBuilder.UpdateData("Subjects", "SubjectCode", 7, "Symbol", "🕵");
            migrationBuilder.UpdateData("Subjects", "SubjectCode", 8, "Symbol", "💡");
            migrationBuilder.UpdateData("Subjects", "SubjectCode", 9, "Symbol", "🐟");
            migrationBuilder.UpdateData("Subjects", "SubjectCode", 10, "Symbol", "🧠");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Symbol",
                table: "Subjects");
        }
    }
}
