using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddUserGuideTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserGuideContentTree",
                columns: table => new
                {
                    NodeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGuideContentTree", x => x.NodeId);
                    table.ForeignKey(
                        name: "FK_UserGuideContentTree_UserGuideContentTree_ParentId",
                        column: x => x.ParentId,
                        principalTable: "UserGuideContentTree",
                        principalColumn: "NodeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserGuideArticles",
                columns: table => new
                {
                    UserGuideArticleId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserGuideContentTreeId = table.Column<Guid>(type: "uuid", nullable: false),
                    HtmlText = table.Column<string>(type: "text", nullable: true),
                    VideoLink = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGuideArticles", x => x.UserGuideArticleId);
                    table.ForeignKey(
                        name: "FK_UserGuideArticles_UserGuideContentTree_UserGuideContentTree~",
                        column: x => x.UserGuideContentTreeId,
                        principalTable: "UserGuideContentTree",
                        principalColumn: "NodeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserGuideArticles_UserGuideContentTreeId",
                table: "UserGuideArticles",
                column: "UserGuideContentTreeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserGuideContentTree_ParentId",
                table: "UserGuideContentTree",
                column: "ParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserGuideArticles");

            migrationBuilder.DropTable(
                name: "UserGuideContentTree");
        }
    }
}
