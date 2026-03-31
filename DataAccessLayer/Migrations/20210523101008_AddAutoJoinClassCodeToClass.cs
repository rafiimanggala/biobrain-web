using Microsoft.EntityFrameworkCore.Migrations;


namespace DataAccessLayer.Migrations
{
    public partial class AddAutoJoinClassCodeToClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AutoJoinClassCode",
                table: "SchoolClasses",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchoolClasses_AutoJoinClassCode",
                table: "SchoolClasses",
                column: "AutoJoinClassCode",
                unique: true);

            migrationBuilder.Sql("UPDATE \"SchoolClasses\" SET \"AutoJoinClassCode\" = substring(upper(md5(random()::text || clock_timestamp()::text)) from 1 for 6)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SchoolClasses_AutoJoinClassCode",
                table: "SchoolClasses");

            migrationBuilder.DropColumn(
                name: "AutoJoinClassCode",
                table: "SchoolClasses");
        }
    }
}
