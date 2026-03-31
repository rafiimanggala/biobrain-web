using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizSoundEnabled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SamlCertificate",
                table: "Schools",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SamlEntityId",
                table: "Schools",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SamlLoginUrl",
                table: "Schools",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SamlMetadataUrl",
                table: "Schools",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SsoEnabled",
                table: "Schools",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoundEnabled",
                table: "QuizAssignments",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "WhatsNew",
                columns: table => new
                {
                    WhatsNewId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhatsNew", x => x.WhatsNewId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WhatsNew");

            migrationBuilder.DropColumn(
                name: "SamlCertificate",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "SamlEntityId",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "SamlLoginUrl",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "SamlMetadataUrl",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "SsoEnabled",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "SoundEnabled",
                table: "QuizAssignments");
        }
    }
}
