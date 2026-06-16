using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecialOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpecialOperationLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperationType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    PerformedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    TargetUserId = table.Column<long>(type: "bigint", nullable: true),
                    RelatedTicketId = table.Column<long>(type: "bigint", nullable: true),
                    RelatedOrderId = table.Column<long>(type: "bigint", nullable: true),
                    RelatedEventId = table.Column<long>(type: "bigint", nullable: true),
                    RelatedWalletTransactionId = table.Column<long>(type: "bigint", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SupportTicketNumber = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    RequestPayloadJson = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    PreviewPayloadJson = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    ResultPayloadJson = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    FailureMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialOperationLogs", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "BalanceTransactionTypes",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "DisplayNameFa", "DisplayOrder", "IsActive", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 14L, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), null, "شارژ دستی کیف پول", 14, true, false, "ManualWalletCredit", null },
                    { 15L, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), null, "کسر دستی کیف پول", 15, true, false, "ManualWalletDebit", null },
                    { 16L, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), null, "کسر کیف پول بابت صدور دستی بلیت", 16, true, false, "ManualTicketPurchaseDebit", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpecialOperationLogs_IdempotencyKey",
                table: "SpecialOperationLogs",
                column: "IdempotencyKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpecialOperationLogs_OperationType_CreatedAt",
                table: "SpecialOperationLogs",
                columns: new[] { "OperationType", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SpecialOperationLogs_PerformedByUserId",
                table: "SpecialOperationLogs",
                column: "PerformedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialOperationLogs_RelatedEventId",
                table: "SpecialOperationLogs",
                column: "RelatedEventId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialOperationLogs_RelatedOrderId",
                table: "SpecialOperationLogs",
                column: "RelatedOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialOperationLogs_RelatedTicketId",
                table: "SpecialOperationLogs",
                column: "RelatedTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialOperationLogs_RelatedWalletTransactionId",
                table: "SpecialOperationLogs",
                column: "RelatedWalletTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialOperationLogs_Status_CreatedAt",
                table: "SpecialOperationLogs",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SpecialOperationLogs_TargetUserId",
                table: "SpecialOperationLogs",
                column: "TargetUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpecialOperationLogs");

            migrationBuilder.DeleteData(
                table: "BalanceTransactionTypes",
                keyColumn: "Id",
                keyValue: 14L);

            migrationBuilder.DeleteData(
                table: "BalanceTransactionTypes",
                keyColumn: "Id",
                keyValue: 15L);

            migrationBuilder.DeleteData(
                table: "BalanceTransactionTypes",
                keyColumn: "Id",
                keyValue: 16L);
        }
    }
}
