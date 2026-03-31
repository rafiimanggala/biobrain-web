using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddTemplates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.TemplateId);
                });

            migrationBuilder.CreateTable(
                name: "CourseTemplates",
                columns: table => new
                {
                    CourseTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseTemplates", x => x.CourseTemplateId);
                    table.ForeignKey(
                        name: "FK_CourseTemplates_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseTemplates_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Templates",
                        principalColumn: "TemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseTemplates_CourseId",
                table: "CourseTemplates",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTemplates_TemplateId",
                table: "CourseTemplates",
                column: "TemplateId");

            migrationBuilder.InsertData("Templates", new string[] { "TemplateId", "Type", "Value"},
	            new object[,]
	            {
		            { Guid.Parse("1BC16A3B-FA9D-4EE9-B6FF-FB0B5E886A57"), 1, "<div>Unit {0:i} AOS {1:i}</div><div>{3}</div><div>Level {4:i}</div>" },
		            { Guid.Parse("D19D9A60-3F5E-4CB8-8D07-B165AE6D1CD0"), 1, "<div>{0}</div><div>{1}</div><div>Level {2:i}</div>" }
	            });


            migrationBuilder.InsertData("CourseTemplates", new string[] { "CourseTemplateId", "TemplateId", "CourseId" },
	            new object[,]
	            {
                    // Generic
		            { Guid.Parse("2008994C-9EF6-4E7E-AA19-9217CA139668"), Guid.Parse("D19D9A60-3F5E-4CB8-8D07-B165AE6D1CD0"), Guid.Parse("16d4754f-4219-4271-b6ce-ee563ee9c0a5") },
		            { Guid.Parse("D167CDA9-886E-4504-882F-7807A3AEFDC7"), Guid.Parse("D19D9A60-3F5E-4CB8-8D07-B165AE6D1CD0"), Guid.Parse("8e698e20-7961-4a81-a62f-e8255f6cfa60") },
		            { Guid.Parse("E65323EF-39EA-4861-8FEF-4C7DBBA0C89A"), Guid.Parse("D19D9A60-3F5E-4CB8-8D07-B165AE6D1CD0"), Guid.Parse("aa91b5fb-b93d-489f-901b-63c4519d63e0") },

                    // VCE
                    { Guid.Parse("08A64EA0-00AA-48D3-9176-913B41E62DDD"), Guid.Parse("1BC16A3B-FA9D-4EE9-B6FF-FB0B5E886A57"), Guid.Parse("6553e323-828f-44e2-bfa1-38584cb056f0") },
                    { Guid.Parse("C605FA46-048C-4C38-B6F6-48FC1DE9B16D"), Guid.Parse("1BC16A3B-FA9D-4EE9-B6FF-FB0B5E886A57"), Guid.Parse("a00e14ab-adc8-40b4-ac03-6df1fc9a9b46") },
                    { Guid.Parse("126E121E-1083-4660-B663-553F8C71667C"), Guid.Parse("1BC16A3B-FA9D-4EE9-B6FF-FB0B5E886A57"), Guid.Parse("766ef33f-598f-4350-a691-2e27ca25b84d") },
                    { Guid.Parse("327FA0EF-2733-41F9-858E-87921B0BADD7"), Guid.Parse("1BC16A3B-FA9D-4EE9-B6FF-FB0B5E886A57"), Guid.Parse("26ced180-9a6d-47a2-93d8-4e3731628d09") },
                    { Guid.Parse("4316D25E-8A84-47F2-B670-602DCD092DFF"), Guid.Parse("1BC16A3B-FA9D-4EE9-B6FF-FB0B5E886A57"), Guid.Parse("daf966ab-774e-4d52-a806-ff7241217a02") },
                    { Guid.Parse("77F3C92F-5B95-46DE-B7C7-5DA214BDE90A"), Guid.Parse("1BC16A3B-FA9D-4EE9-B6FF-FB0B5E886A57"), Guid.Parse("3e82da8e-5034-493d-a51b-87098bb35fbb") },

                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseTemplates");

            migrationBuilder.DropTable(
                name: "Templates");
        }
    }
}
