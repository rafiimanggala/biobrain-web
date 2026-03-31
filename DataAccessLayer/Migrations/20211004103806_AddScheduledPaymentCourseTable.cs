using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddScheduledPaymentCourseTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduledPaymentCourse",
                columns: table => new
                {
                    ScheduledPaymentCourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledPaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledPaymentCourse", x => x.ScheduledPaymentCourseId);
                    table.ForeignKey(
                        name: "FK_ScheduledPaymentCourse_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduledPaymentCourse_ScheduledPayment_ScheduledPaymentId",
                        column: x => x.ScheduledPaymentId,
                        principalTable: "ScheduledPayment",
                        principalColumn: "ScheduledPaymentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledPaymentCourse_CourseId",
                table: "ScheduledPaymentCourse",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledPaymentCourse_ScheduledPaymentId",
                table: "ScheduledPaymentCourse",
                column: "ScheduledPaymentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduledPaymentCourse");
        }
    }
}
