using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddUserPageViewsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSession_AspNetUsers_UserId",
                table: "UserSession");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSessionCourse_Courses_CourseId",
                table: "UserSessionCourse");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSessionCourse_UserSession_UserSessionId",
                table: "UserSessionCourse");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSessionSchool_Schools_SchoolId",
                table: "UserSessionSchool");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSessionSchool_UserSession_UserSessionId",
                table: "UserSessionSchool");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSessionSchool",
                table: "UserSessionSchool");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSessionCourse",
                table: "UserSessionCourse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSession",
                table: "UserSession");

            migrationBuilder.RenameTable(
                name: "UserSessionSchool",
                newName: "UserSessionSchools");

            migrationBuilder.RenameTable(
                name: "UserSessionCourse",
                newName: "UserSessionCourses");

            migrationBuilder.RenameTable(
                name: "UserSession",
                newName: "UserSessions");

            migrationBuilder.RenameIndex(
                name: "IX_UserSessionSchool_UserSessionId",
                table: "UserSessionSchools",
                newName: "IX_UserSessionSchools_UserSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSessionSchool_SchoolId",
                table: "UserSessionSchools",
                newName: "IX_UserSessionSchools_SchoolId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSessionCourse_UserSessionId",
                table: "UserSessionCourses",
                newName: "IX_UserSessionCourses_UserSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSessionCourse_CourseId",
                table: "UserSessionCourses",
                newName: "IX_UserSessionCourses_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSession_UserId",
                table: "UserSessions",
                newName: "IX_UserSessions_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSessionSchools",
                table: "UserSessionSchools",
                column: "UserSessionSchoolId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSessionCourses",
                table: "UserSessionCourses",
                column: "UserSessionCourseEntityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSessions",
                table: "UserSessions",
                column: "UserSessionId");

            migrationBuilder.CreateTable(
                name: "UserPageViews",
                columns: table => new
                {
                    UserPageViewId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchoolId = table.Column<Guid>(type: "uuid", nullable: true),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPageViews", x => x.UserPageViewId);
                    table.ForeignKey(
                        name: "FK_UserPageViews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPageViews_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPageViews_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "SchoolId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPageViews_CourseId",
                table: "UserPageViews",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPageViews_SchoolId",
                table: "UserPageViews",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPageViews_UserId",
                table: "UserPageViews",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessionCourses_Courses_CourseId",
                table: "UserSessionCourses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessionCourses_UserSessions_UserSessionId",
                table: "UserSessionCourses",
                column: "UserSessionId",
                principalTable: "UserSessions",
                principalColumn: "UserSessionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessions_AspNetUsers_UserId",
                table: "UserSessions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessionSchools_Schools_SchoolId",
                table: "UserSessionSchools",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "SchoolId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessionSchools_UserSessions_UserSessionId",
                table: "UserSessionSchools",
                column: "UserSessionId",
                principalTable: "UserSessions",
                principalColumn: "UserSessionId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSessionCourses_Courses_CourseId",
                table: "UserSessionCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSessionCourses_UserSessions_UserSessionId",
                table: "UserSessionCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSessions_AspNetUsers_UserId",
                table: "UserSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSessionSchools_Schools_SchoolId",
                table: "UserSessionSchools");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSessionSchools_UserSessions_UserSessionId",
                table: "UserSessionSchools");

            migrationBuilder.DropTable(
                name: "UserPageViews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSessionSchools",
                table: "UserSessionSchools");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSessions",
                table: "UserSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSessionCourses",
                table: "UserSessionCourses");

            migrationBuilder.RenameTable(
                name: "UserSessionSchools",
                newName: "UserSessionSchool");

            migrationBuilder.RenameTable(
                name: "UserSessions",
                newName: "UserSession");

            migrationBuilder.RenameTable(
                name: "UserSessionCourses",
                newName: "UserSessionCourse");

            migrationBuilder.RenameIndex(
                name: "IX_UserSessionSchools_UserSessionId",
                table: "UserSessionSchool",
                newName: "IX_UserSessionSchool_UserSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSessionSchools_SchoolId",
                table: "UserSessionSchool",
                newName: "IX_UserSessionSchool_SchoolId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSession",
                newName: "IX_UserSession_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSessionCourses_UserSessionId",
                table: "UserSessionCourse",
                newName: "IX_UserSessionCourse_UserSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSessionCourses_CourseId",
                table: "UserSessionCourse",
                newName: "IX_UserSessionCourse_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSessionSchool",
                table: "UserSessionSchool",
                column: "UserSessionSchoolId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSession",
                table: "UserSession",
                column: "UserSessionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSessionCourse",
                table: "UserSessionCourse",
                column: "UserSessionCourseEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSession_AspNetUsers_UserId",
                table: "UserSession",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessionCourse_Courses_CourseId",
                table: "UserSessionCourse",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessionCourse_UserSession_UserSessionId",
                table: "UserSessionCourse",
                column: "UserSessionId",
                principalTable: "UserSession",
                principalColumn: "UserSessionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessionSchool_Schools_SchoolId",
                table: "UserSessionSchool",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "SchoolId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessionSchool_UserSession_UserSessionId",
                table: "UserSessionSchool",
                column: "UserSessionId",
                principalTable: "UserSession",
                principalColumn: "UserSessionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
