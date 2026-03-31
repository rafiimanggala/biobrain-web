using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddUserVoucherTransactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Update \"UserVouchers\" SET \"UserVoucherId\" = uuid_generate_v4()");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserVouchers",
                table: "UserVouchers");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserVouchers",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserVouchers",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserVouchers",
                table: "UserVouchers",
                column: "UserVoucherId");

            migrationBuilder.CreateTable(
                name: "UserVoucherTransactions",
                columns: table => new
                {
                    UserVoucherTransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    UserVoucherId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVoucherTransactions", x => x.UserVoucherTransactionId);
                    table.ForeignKey(
                        name: "FK_UserVoucherTransactions_UserVouchers_UserVoucherId",
                        column: x => x.UserVoucherId,
                        principalTable: "UserVouchers",
                        principalColumn: "UserVoucherId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserVouchers_VoucherId",
                table: "UserVouchers",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVoucherTransactions_UserVoucherId",
                table: "UserVoucherTransactions",
                column: "UserVoucherId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserVoucherTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserVouchers",
                table: "UserVouchers");

            migrationBuilder.DropIndex(
                name: "IX_UserVouchers_VoucherId",
                table: "UserVouchers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserVouchers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UserVouchers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserVouchers",
                table: "UserVouchers",
                column: "VoucherId");
        }
    }
}
