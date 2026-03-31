using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddStartIndexForContentTreeMeta : Migration
    {
	    protected override void Up(MigrationBuilder migrationBuilder)
	    {
		    migrationBuilder.AddColumn<int>(
			    name: "StartIndex",
			    table: "ContentTreeMeta",
			    type: "integer",
			    nullable: false,
			    defaultValue: 0);

		    migrationBuilder.UpdateData("ContentTreeMeta", "ContentTreeMetaId", "9575FD04-0E50-4D08-BE4A-1F85F7EDA46F",
			    "StartIndex", 2);
		    migrationBuilder.UpdateData("ContentTreeMeta", "ContentTreeMetaId", "C67D52FB-FCFF-4F0B-BA46-9B782F4529C6",
			    "StartIndex", 2);
		    migrationBuilder.UpdateData("ContentTreeMeta", "ContentTreeMetaId", "CDC844CD-9234-470A-BE57-B7E7C7F97A00",
			    "StartIndex", 2);
		    migrationBuilder.UpdateData("ContentTreeMeta", "ContentTreeMetaId", "491FC325-F62E-4943-BD74-73EBAB0BFE1B",
			    "StartIndex", 2);
		    migrationBuilder.UpdateData("ContentTreeMeta", "ContentTreeMetaId", "AF4BDC3F-29C9-419D-80D1-69D681AC2059",
			    "StartIndex", 2);
		    migrationBuilder.UpdateData("ContentTreeMeta", "ContentTreeMetaId", "C6CE6CAB-C2CD-4528-B465-2E4A1EEDFA45",
			    "StartIndex", 2);
	    }

	    protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartIndex",
                table: "ContentTreeMeta");
        }
    }
}
