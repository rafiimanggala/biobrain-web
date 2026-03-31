using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddTablesForAccessCodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessCodeBatches",
                columns: table => new
                {
                    AccessCodeBatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    NumberOfCodes = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessCodeBatches", x => x.AccessCodeBatchId);
                });

            migrationBuilder.CreateTable(
                name: "AccessCodeBatchCourses",
                columns: table => new
                {
                    AccessCodeCourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessCodeBatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessCodeBatchCourses", x => x.AccessCodeCourseId);
                    table.ForeignKey(
                        name: "FK_AccessCodeBatchCourses_AccessCodeBatches_AccessCodeBatchId",
                        column: x => x.AccessCodeBatchId,
                        principalTable: "AccessCodeBatches",
                        principalColumn: "AccessCodeBatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessCodeBatchCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccessCodes",
                columns: table => new
                {
                    AccessCodeId = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessCodes", x => x.AccessCodeId);
                    table.ForeignKey(
                        name: "FK_AccessCodes_AccessCodeBatches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "AccessCodeBatches",
                        principalColumn: "AccessCodeBatchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessCodeBatchCourses_AccessCodeBatchId",
                table: "AccessCodeBatchCourses",
                column: "AccessCodeBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessCodeBatchCourses_CourseId",
                table: "AccessCodeBatchCourses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessCodes_BatchId",
                table: "AccessCodes",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessCodes_Code",
                table: "AccessCodes",
                column: "Code",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessCodeBatchCourses");

            migrationBuilder.DropTable(
                name: "AccessCodes");

            migrationBuilder.DropTable(
                name: "AccessCodeBatches");
        }
    }
}
