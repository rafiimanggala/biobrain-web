using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddUserSessionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSession",
                columns: table => new
                {
                    UserSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastTrack = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSession", x => x.UserSessionId);
                    table.ForeignKey(
                        name: "FK_UserSession_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSessionCourse",
                columns: table => new
                {
                    UserSessionCourseEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessionCourse", x => x.UserSessionCourseEntityId);
                    table.ForeignKey(
                        name: "FK_UserSessionCourse_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSessionCourse_UserSession_UserSessionId",
                        column: x => x.UserSessionId,
                        principalTable: "UserSession",
                        principalColumn: "UserSessionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSessionSchool",
                columns: table => new
                {
                    UserSessionSchoolId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchoolId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessionSchool", x => x.UserSessionSchoolId);
                    table.ForeignKey(
                        name: "FK_UserSessionSchool_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "SchoolId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSessionSchool_UserSession_UserSessionId",
                        column: x => x.UserSessionId,
                        principalTable: "UserSession",
                        principalColumn: "UserSessionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSession_UserId",
                table: "UserSession",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessionCourse_CourseId",
                table: "UserSessionCourse",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessionCourse_UserSessionId",
                table: "UserSessionCourse",
                column: "UserSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessionSchool_SchoolId",
                table: "UserSessionSchool",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessionSchool_UserSessionId",
                table: "UserSessionSchool",
                column: "UserSessionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSessionCourse");

            migrationBuilder.DropTable(
                name: "UserSessionSchool");

            migrationBuilder.DropTable(
                name: "UserSession");
        }
    }
}
