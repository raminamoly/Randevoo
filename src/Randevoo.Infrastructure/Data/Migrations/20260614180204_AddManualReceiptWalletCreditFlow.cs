using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddManualReceiptWalletCreditFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "WalletCreditTransactionId",
                table: "ManualPaymentReceipts",
                type: "bigint",
                nullable: true);

            migrationBuilder.InsertData(
                table: "BalanceTransactionTypes",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "DisplayNameFa", "DisplayOrder", "IsActive", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 12L, new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), null, "اعتبار کیف پول بابت رسید دستی", 12, true, false, "ManualReceiptWalletCredit", null },
                    { 13L, new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), null, "بدهی برگزارکننده بابت رسید دستی", 13, true, false, "OrganizerManualReceiptLiability", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ManualPaymentReceipts_WalletCreditTransactionId",
                table: "ManualPaymentReceipts",
                column: "WalletCreditTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ManualPaymentReceipts_BalanceTransactions_WalletCreditTransactionId",
                table: "ManualPaymentReceipts",
                column: "WalletCreditTransactionId",
                principalTable: "BalanceTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManualPaymentReceipts_BalanceTransactions_WalletCreditTransactionId",
                table: "ManualPaymentReceipts");

            migrationBuilder.DropIndex(
                name: "IX_ManualPaymentReceipts_WalletCreditTransactionId",
                table: "ManualPaymentReceipts");

            migrationBuilder.DeleteData(
                table: "BalanceTransactionTypes",
                keyColumn: "Id",
                keyValue: 12L);

            migrationBuilder.DeleteData(
                table: "BalanceTransactionTypes",
                keyColumn: "Id",
                keyValue: 13L);

            migrationBuilder.DropColumn(
                name: "WalletCreditTransactionId",
                table: "ManualPaymentReceipts");
        }
    }
}
