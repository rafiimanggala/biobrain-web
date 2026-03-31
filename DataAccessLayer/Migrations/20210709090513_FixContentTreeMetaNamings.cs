using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class FixContentTreeMetaNamings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder){
	        migrationBuilder.Sql("UPDATE \"ContentTreeMeta\" SET \"Name\" = 'Key Knowledge' WHERE \"Name\" LIKE 'Key Knoledge'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
