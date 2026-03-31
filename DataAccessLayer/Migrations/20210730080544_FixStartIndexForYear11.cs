using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class FixStartIndexForYear11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.UpdateData("ContentTreeMeta", "ContentTreeMetaId", "491FC325-F62E-4943-BD74-73EBAB0BFE1B",
		        "StartIndex", 0);
	        migrationBuilder.UpdateData("ContentTreeMeta", "ContentTreeMetaId", "AF4BDC3F-29C9-419D-80D1-69D681AC2059",
		        "StartIndex", 0);
	        migrationBuilder.UpdateData("ContentTreeMeta", "ContentTreeMetaId", "C6CE6CAB-C2CD-4528-B465-2E4A1EEDFA45",
		        "StartIndex", 0);
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
