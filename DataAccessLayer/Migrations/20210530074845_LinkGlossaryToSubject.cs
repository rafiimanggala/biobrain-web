using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class LinkGlossaryToSubject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubjectCode",
                table: "GlossaryTerms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                UPDATE ""GlossaryTerms"" AS GT
                SET ""SubjectCode"" = S.""SubjectCode""
                FROM ""Subjects"" S
                JOIN ""Courses"" C ON S.""SubjectCode"" = C.""SubjectCode""
                WHERE GT.""CourseId"" = C.""CourseId"";"
            );

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "GlossaryTerms");

            migrationBuilder.CreateIndex(
                name: "IX_GlossaryTerms_SubjectCode",
                table: "GlossaryTerms",
                column: "SubjectCode");

            migrationBuilder.AddForeignKey(
                name: "FK_GlossaryTerms_Subjects_SubjectCode",
                table: "GlossaryTerms",
                column: "SubjectCode",
                principalTable: "Subjects",
                principalColumn: "SubjectCode",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlossaryTerms_Subjects_SubjectCode",
                table: "GlossaryTerms");

            migrationBuilder.DropIndex(
                name: "IX_GlossaryTerms_SubjectCode",
                table: "GlossaryTerms");

            migrationBuilder.DropColumn(
                name: "SubjectCode",
                table: "GlossaryTerms");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "GlossaryTerms",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
