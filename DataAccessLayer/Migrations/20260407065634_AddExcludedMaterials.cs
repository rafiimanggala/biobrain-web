using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddExcludedMaterials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExcludedMaterials",
                columns: table => new
                {
                    ExcludedMaterialId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchoolClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExcludedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExcludedMaterials", x => x.ExcludedMaterialId);
                    table.ForeignKey(
                        name: "FK_ExcludedMaterials_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "MaterialId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExcludedMaterials_SchoolClasses_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "SchoolClasses",
                        principalColumn: "SchoolClassId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExcludedMaterials_MaterialId",
                table: "ExcludedMaterials",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_ExcludedMaterials_SchoolClassId_MaterialId",
                table: "ExcludedMaterials",
                columns: new[] { "SchoolClassId", "MaterialId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExcludedMaterials");
        }
    }
}
