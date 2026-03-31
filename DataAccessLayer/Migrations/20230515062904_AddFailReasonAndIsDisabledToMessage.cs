using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddFailReasonAndIsDisabledToMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FailReason",
                table: "EmailMessageQueue",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDisabled",
                table: "EmailMessageQueue",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailReason",
                table: "EmailMessageQueue");

            migrationBuilder.DropColumn(
                name: "IsDisabled",
                table: "EmailMessageQueue");
        }
    }
}
