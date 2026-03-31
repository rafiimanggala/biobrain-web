using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddIsForSellToCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsForSell",
                table: "Courses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData("Courses", "CourseId", Guid.Parse("6553E323-828F-44E2-BFA1-38584CB056F0"), "IsForSell", true);
            migrationBuilder.UpdateData("Courses", "CourseId", Guid.Parse("A00E14AB-ADC8-40B4-AC03-6DF1FC9A9B46"), "IsForSell", true);
            migrationBuilder.UpdateData("Courses", "CourseId", Guid.Parse("766EF33F-598F-4350-A691-2E27CA25B84D"), "IsForSell", true);

            migrationBuilder.UpdateData("Courses", "CourseId", Guid.Parse("26CED180-9A6D-47A2-93D8-4E3731628D09"), "IsForSell", true);
            migrationBuilder.UpdateData("Courses", "CourseId", Guid.Parse("DAF966AB-774E-4D52-A806-FF7241217A02"), "IsForSell", true);
            migrationBuilder.UpdateData("Courses", "CourseId", Guid.Parse("3E82DA8E-5034-493D-A51B-87098BB35FBB"), "IsForSell", true);

            migrationBuilder.UpdateData("Courses", "CourseId", Guid.Parse("16d4754f-4219-4271-b6ce-ee563ee9c0a5"), "IsForSell", true);
            migrationBuilder.UpdateData("Courses", "CourseId", Guid.Parse("8e698e20-7961-4a81-a62f-e8255f6cfa60"), "IsForSell", true);
            migrationBuilder.UpdateData("Courses", "CourseId", Guid.Parse("aa91b5fb-b93d-489f-901b-63c4519d63e0"), "IsForSell", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsForSell",
                table: "Courses");
        }
    }
}
