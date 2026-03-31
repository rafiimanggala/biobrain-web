using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddAccessCodeMilestoneTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessCodeMilestone",
                columns: table => new
                {
                    AccessCodeId = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessCodeMilestone", x => x.AccessCodeId);
                    table.ForeignKey(
                        name: "FK_AccessCodeMilestone_AccessCodeBatches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "AccessCodeBatches",
                        principalColumn: "AccessCodeBatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessCodeMilestone_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessCodeMilestone_BatchId",
                table: "AccessCodeMilestone",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessCodeMilestone_UserId",
                table: "AccessCodeMilestone",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessCodeMilestone");
        }
    }
}
