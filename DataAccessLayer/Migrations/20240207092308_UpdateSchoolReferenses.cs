using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class UpdateSchoolReferenses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPageViews_Schools_SchoolId",
                table: "UserPageViews");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSessionSchools_Schools_SchoolId",
                table: "UserSessionSchools");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPageViews_Schools_SchoolId",
                table: "UserPageViews",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "SchoolId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessionSchools_Schools_SchoolId",
                table: "UserSessionSchools",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "SchoolId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPageViews_Schools_SchoolId",
                table: "UserPageViews");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSessionSchools_Schools_SchoolId",
                table: "UserSessionSchools");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPageViews_Schools_SchoolId",
                table: "UserPageViews",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "SchoolId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessionSchools_Schools_SchoolId",
                table: "UserSessionSchools",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "SchoolId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
