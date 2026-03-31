using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class UpdateUsersAndPayments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TimeZoneId",
                table: "Teachers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeZoneId",
                table: "Students",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LastPaymentReview",
                columns: table => new
                {
                    LastPaymentReviewId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PayDate = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastPaymentReview", x => x.LastPaymentReviewId);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledPayment",
                columns: table => new
                {
                    ScheduledPaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Period = table.Column<int>(type: "integer", nullable: false),
                    PayDate = table.Column<int>(type: "integer", nullable: false),
                    LeapPayDate = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledPayment", x => x.ScheduledPaymentId);
                    table.ForeignKey(
                        name: "FK_ScheduledPayment_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPaymentDetails",
                columns: table => new
                {
                    UserPaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    PinPaymentCustomerRefId = table.Column<string>(type: "text", nullable: true),
                    IpAddress = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPaymentDetails", x => x.UserPaymentId);
                    table.ForeignKey(
                        name: "FK_UserPaymentDetails_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LastPaidScheduledPayment",
                columns: table => new
                {
                    LastPaidScheduledPaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ScheduledPaymentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastPaidScheduledPayment", x => x.LastPaidScheduledPaymentId);
                    table.ForeignKey(
                        name: "FK_LastPaidScheduledPayment_ScheduledPayment_ScheduledPaymentId",
                        column: x => x.ScheduledPaymentId,
                        principalTable: "ScheduledPayment",
                        principalColumn: "ScheduledPaymentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ScheduledPaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ChargeRefId = table.Column<string>(type: "text", nullable: true),
                    TransferRefId = table.Column<string>(type: "text", nullable: true),
                    FailedPayload = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payment_ScheduledPayment_ScheduledPaymentId",
                        column: x => x.ScheduledPaymentId,
                        principalTable: "ScheduledPayment",
                        principalColumn: "ScheduledPaymentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LastPaidScheduledPayment_ScheduledPaymentId",
                table: "LastPaidScheduledPayment",
                column: "ScheduledPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_ScheduledPaymentId",
                table: "Payment",
                column: "ScheduledPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledPayment_UserId",
                table: "ScheduledPayment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPaymentDetails_UserId",
                table: "UserPaymentDetails",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LastPaidScheduledPayment");

            migrationBuilder.DropTable(
                name: "LastPaymentReview");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "UserPaymentDetails");

            migrationBuilder.DropTable(
                name: "ScheduledPayment");

            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                table: "Students");
        }
    }
}
