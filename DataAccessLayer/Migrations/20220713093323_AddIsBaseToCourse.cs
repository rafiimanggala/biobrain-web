using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddIsBaseToCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBase",
                table: "Courses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // Bio - Generic
            migrationBuilder.UpdateData("Courses", "CourseId", "16d4754f-4219-4271-b6ce-ee563ee9c0a5", "IsBase", true);
            // Chem - Generic
            migrationBuilder.UpdateData("Courses", "CourseId", "8e698e20-7961-4a81-a62f-e8255f6cfa60", "IsBase", true);
            // Phy - Generic
            migrationBuilder.UpdateData("Courses", "CourseId", "aa91b5fb-b93d-489f-901b-63c4519d63e0", "IsBase", true);
            // Bio - 10
            migrationBuilder.UpdateData("Courses", "CourseId", "cb628740-a88c-4ccb-addc-32307dbb1659", "IsBase", true);
            // Chem - 10
            migrationBuilder.UpdateData("Courses", "CourseId", "22644304-9de3-4493-88a2-c27fe004c35b", "IsBase", true);
            // Phy - 10
            migrationBuilder.UpdateData("Courses", "CourseId", "af5a7db1-d7ee-4d4d-9d6a-d237886dda75", "IsBase", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBase",
                table: "Courses");
        }
    }
}
