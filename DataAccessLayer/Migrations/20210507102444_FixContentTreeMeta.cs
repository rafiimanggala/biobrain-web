using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class FixContentTreeMeta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.UpdateData("ContentTreeMeta", "ContentTreeMetaId", Guid.Parse("D17244DF-AB06-4443-ABBA-FE1A14BFF3A9"), "CouldAddEntry", false);
	        migrationBuilder.UpdateData("ContentTreeMeta", "ContentTreeMetaId", Guid.Parse("E8753AA5-2939-48FD-AB9D-A186F211338F"), "CouldAddEntry", false);
	        migrationBuilder.UpdateData("ContentTreeMeta", "ContentTreeMetaId", Guid.Parse("80297E95-C6EC-4D2B-8E71-9C999A1CAFDA"), "CouldAddEntry", false);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
