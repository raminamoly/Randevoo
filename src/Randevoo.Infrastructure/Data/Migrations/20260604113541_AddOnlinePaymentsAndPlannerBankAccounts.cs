using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOnlinePaymentsAndPlannerBankAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OnlinePayments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    DatingEventId = table.Column<long>(type: "bigint", nullable: true),
                    EventTicketId = table.Column<long>(type: "bigint", nullable: true),
                    BalanceTransactionId = table.Column<long>(type: "bigint", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GatewayName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    TrackingCode = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaidAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnlinePayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnlinePayments_BalanceTransactions_BalanceTransactionId",
                        column: x => x.BalanceTransactionId,
                        principalTable: "BalanceTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OnlinePayments_DatingEvents_DatingEventId",
                        column: x => x.DatingEventId,
                        principalTable: "DatingEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OnlinePayments_EventTickets_EventTicketId",
                        column: x => x.EventTicketId,
                        principalTable: "EventTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OnlinePayments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlannerBankAccounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(19)", maxLength: 19, nullable: false),
                    Iban = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlannerBankAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlannerBankAccounts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OnlinePayments_BalanceTransactionId",
                table: "OnlinePayments",
                column: "BalanceTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_OnlinePayments_DatingEventId",
                table: "OnlinePayments",
                column: "DatingEventId");

            migrationBuilder.CreateIndex(
                name: "IX_OnlinePayments_EventTicketId",
                table: "OnlinePayments",
                column: "EventTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_OnlinePayments_TrackingCode",
                table: "OnlinePayments",
                column: "TrackingCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OnlinePayments_UserId",
                table: "OnlinePayments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlannerBankAccounts_Iban",
                table: "PlannerBankAccounts",
                column: "Iban",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlannerBankAccounts_UserId",
                table: "PlannerBankAccounts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OnlinePayments");

            migrationBuilder.DropTable(
                name: "PlannerBankAccounts");
        }
    }
}
