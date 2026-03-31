using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddContentVersionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentVersion",
                columns: table => new
                {
                    ContentVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    MajorVersion = table.Column<long>(type: "bigint", nullable: false),
                    MinorVersion = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentVersion", x => x.ContentVersionId);
                });

            migrationBuilder.InsertData("ContentVersion",
	            new string[] {"ContentVersionId", "MajorVersion", "MinorVersion", "CreatedAt", "UpdatedAt"},
	            new object[,]
		            {{Guid.Parse("D9251821-83C0-4B3D-B140-656553C47EE7"), 1, 1, DateTime.UtcNow, DateTime.UtcNow}});
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentVersion");
        }
    }
}
