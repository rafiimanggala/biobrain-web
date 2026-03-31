using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddLastContentUpdateToCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastContentUpdateUtc",
                table: "Courses",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(2022, 7, 14, 0, 0, 0, 0));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastContentUpdateUtc",
                table: "Courses");
        }
    }
}
