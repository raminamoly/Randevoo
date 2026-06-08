using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencySettlementAndExchangeRates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlannerBankAccounts_Iban",
                table: "PlannerBankAccounts");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "PlannerWithdrawalRequests",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "IRR");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExchangeRateCapturedAtUtc",
                table: "PlannerWithdrawalRequests",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<long>(
                name: "ExchangeRateId",
                table: "PlannerWithdrawalRequests",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRateToIrr",
                table: "PlannerWithdrawalRequests",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 1m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReportingAmountIrr",
                table: "PlannerWithdrawalRequests",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "Iban",
                table: "PlannerBankAccounts",
                type: "nvarchar(34)",
                maxLength: 34,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(34)",
                oldMaxLength: 34);

            migrationBuilder.AlterColumn<string>(
                name: "CardNumber",
                table: "PlannerBankAccounts",
                type: "nvarchar(19)",
                maxLength: 19,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(19)",
                oldMaxLength: 19);

            migrationBuilder.AlterColumn<string>(
                name: "BankName",
                table: "PlannerBankAccounts",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AddColumn<string>(
                name: "AccountHolderName",
                table: "PlannerBankAccounts",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "برگزارکننده");

            migrationBuilder.AddColumn<string>(
                name: "AccountIdentifier",
                table: "PlannerBankAccounts",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "PlannerBankAccounts",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "PlannerBankAccounts",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "PlannerBankAccounts",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "IRR");

            migrationBuilder.AddColumn<int>(
                name: "PayoutMethod",
                table: "PlannerBankAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PublicPaymentInstructions",
                table: "PlannerBankAccounts",
                type: "nvarchar(1200)",
                maxLength: 1200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwiftCode",
                table: "PlannerBankAccounts",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "OnlinePayments",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "IRR");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExchangeRateCapturedAtUtc",
                table: "OnlinePayments",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<long>(
                name: "ExchangeRateId",
                table: "OnlinePayments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRateToIrr",
                table: "OnlinePayments",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 1m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReportingAmountIrr",
                table: "OnlinePayments",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExchangeRateCapturedAtUtc",
                table: "EventTickets",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<long>(
                name: "ExchangeRateId",
                table: "EventTickets",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRateToIrr",
                table: "EventTickets",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 1m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReportingOriginalPriceIrr",
                table: "EventTickets",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReportingPriceIrr",
                table: "EventTickets",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SettlementCurrencyCode",
                table: "EventPlannerProfiles",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "IRR");

            migrationBuilder.AddColumn<string>(
                name: "SettlementCurrencyLockReason",
                table: "EventPlannerProfiles",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SettlementCurrencyLockedAtUtc",
                table: "EventPlannerProfiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "BalanceTransactions",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "IRR");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExchangeRateCapturedAtUtc",
                table: "BalanceTransactions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<long>(
                name: "ExchangeRateId",
                table: "BalanceTransactions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRateToIrr",
                table: "BalanceTransactions",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 1m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReportingAmountIrr",
                table: "BalanceTransactions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ReportingCurrencyCode",
                table: "BalanceTransactions",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "IRR");

            migrationBuilder.AddColumn<string>(
                name: "ReportingCurrencyCode",
                table: "BalanceAccounts",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "IRR");

            migrationBuilder.CreateTable(
                name: "CurrencyExchangeRates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromCurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ToCurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    EffectiveFromUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveToUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyExchangeRates", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CurrencyExchangeRates",
                columns: new[] { "Id", "CreatedAt", "CreatedByUserId", "DeletedAt", "EffectiveFromUtc", "EffectiveToUtc", "FromCurrencyCode", "IsDeleted", "Rate", "Source", "ToCurrencyCode", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, "IRR", false, 1m, "Seed", "IRR", null },
                    { 2L, new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, "USD", false, 1750000m, "Seed", "IRR", null },
                    { 3L, new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, "EUR", false, 2000000m, "Seed", "IRR", null },
                    { 4L, new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, "CAD", false, 1280000m, "Seed", "IRR", null },
                    { 5L, new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, "GBP", false, 2350000m, "Seed", "IRR", null },
                    { 6L, new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, "AED", false, 476500m, "Seed", "IRR", null },
                    { 7L, new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, "TRY", false, 54000m, "Seed", "IRR", null }
                });

            migrationBuilder.Sql("""
                UPDATE EventTickets
                SET ReportingPriceIrr = Price,
                    ReportingOriginalPriceIrr = OriginalPrice,
                    ExchangeRateCapturedAtUtc = CreatedAt
                WHERE ReportingPriceIrr = 0 AND IsDeleted = 0;

                UPDATE BalanceTransactions
                SET ReportingAmountIrr = Amount,
                    ExchangeRateCapturedAtUtc = CreatedAt
                WHERE ReportingAmountIrr = 0 AND IsDeleted = 0;

                UPDATE OnlinePayments
                SET ReportingAmountIrr = Amount,
                    ExchangeRateCapturedAtUtc = CreatedAt
                WHERE ReportingAmountIrr = 0 AND IsDeleted = 0;

                UPDATE PlannerWithdrawalRequests
                SET ReportingAmountIrr = Amount,
                    ExchangeRateCapturedAtUtc = CreatedAt
                WHERE ReportingAmountIrr = 0 AND IsDeleted = 0;

                UPDATE EventPlannerProfiles
                SET SettlementCurrencyLockedAtUtc = COALESCE(SettlementCurrencyLockedAtUtc, SYSUTCDATETIME()),
                    SettlementCurrencyLockReason = COALESCE(SettlementCurrencyLockReason, N'Existing event or financial activity before currency foundation')
                WHERE SettlementCurrencyLockedAtUtc IS NULL
                  AND IsDeleted = 0
                  AND (
                    EXISTS (
                        SELECT 1
                        FROM DatingEvents
                        WHERE DatingEvents.EventPlannerUserId = EventPlannerProfiles.UserId
                          AND DatingEvents.IsDeleted = 0
                    )
                    OR EXISTS (
                        SELECT 1
                        FROM BalanceAccounts
                        INNER JOIN BalanceTransactions ON BalanceTransactions.BalanceAccountId = BalanceAccounts.Id
                        WHERE BalanceAccounts.UserId = EventPlannerProfiles.UserId
                          AND BalanceAccounts.IsDeleted = 0
                          AND BalanceTransactions.IsDeleted = 0
                    )
                    OR EXISTS (
                        SELECT 1
                        FROM PlannerWithdrawalRequests
                        WHERE PlannerWithdrawalRequests.UserId = EventPlannerProfiles.UserId
                          AND PlannerWithdrawalRequests.IsDeleted = 0
                    )
                  );
                """);

            migrationBuilder.CreateIndex(
                name: "IX_PlannerWithdrawalRequests_CurrencyCode",
                table: "PlannerWithdrawalRequests",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_PlannerWithdrawalRequests_ExchangeRateId",
                table: "PlannerWithdrawalRequests",
                column: "ExchangeRateId");

            migrationBuilder.CreateIndex(
                name: "IX_PlannerBankAccounts_CurrencyCode",
                table: "PlannerBankAccounts",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_PlannerBankAccounts_Iban",
                table: "PlannerBankAccounts",
                column: "Iban",
                unique: true,
                filter: "[Iban] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OnlinePayments_CurrencyCode",
                table: "OnlinePayments",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_OnlinePayments_ExchangeRateId",
                table: "OnlinePayments",
                column: "ExchangeRateId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTickets_ExchangeRateId",
                table: "EventTickets",
                column: "ExchangeRateId");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceTransactions_CurrencyCode",
                table: "BalanceTransactions",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceTransactions_ExchangeRateId",
                table: "BalanceTransactions",
                column: "ExchangeRateId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_FromCurrencyCode_ToCurrencyCode_EffectiveFromUtc",
                table: "CurrencyExchangeRates",
                columns: new[] { "FromCurrencyCode", "ToCurrencyCode", "EffectiveFromUtc" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_FromCurrencyCode_ToCurrencyCode_EffectiveToUtc",
                table: "CurrencyExchangeRates",
                columns: new[] { "FromCurrencyCode", "ToCurrencyCode", "EffectiveToUtc" });

            migrationBuilder.AddForeignKey(
                name: "FK_BalanceTransactions_CurrencyExchangeRates_ExchangeRateId",
                table: "BalanceTransactions",
                column: "ExchangeRateId",
                principalTable: "CurrencyExchangeRates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventTickets_CurrencyExchangeRates_ExchangeRateId",
                table: "EventTickets",
                column: "ExchangeRateId",
                principalTable: "CurrencyExchangeRates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OnlinePayments_CurrencyExchangeRates_ExchangeRateId",
                table: "OnlinePayments",
                column: "ExchangeRateId",
                principalTable: "CurrencyExchangeRates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlannerWithdrawalRequests_CurrencyExchangeRates_ExchangeRateId",
                table: "PlannerWithdrawalRequests",
                column: "ExchangeRateId",
                principalTable: "CurrencyExchangeRates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BalanceTransactions_CurrencyExchangeRates_ExchangeRateId",
                table: "BalanceTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_EventTickets_CurrencyExchangeRates_ExchangeRateId",
                table: "EventTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_OnlinePayments_CurrencyExchangeRates_ExchangeRateId",
                table: "OnlinePayments");

            migrationBuilder.DropForeignKey(
                name: "FK_PlannerWithdrawalRequests_CurrencyExchangeRates_ExchangeRateId",
                table: "PlannerWithdrawalRequests");

            migrationBuilder.DropTable(
                name: "CurrencyExchangeRates");

            migrationBuilder.DropIndex(
                name: "IX_PlannerWithdrawalRequests_CurrencyCode",
                table: "PlannerWithdrawalRequests");

            migrationBuilder.DropIndex(
                name: "IX_PlannerWithdrawalRequests_ExchangeRateId",
                table: "PlannerWithdrawalRequests");

            migrationBuilder.DropIndex(
                name: "IX_PlannerBankAccounts_CurrencyCode",
                table: "PlannerBankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_PlannerBankAccounts_Iban",
                table: "PlannerBankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_OnlinePayments_CurrencyCode",
                table: "OnlinePayments");

            migrationBuilder.DropIndex(
                name: "IX_OnlinePayments_ExchangeRateId",
                table: "OnlinePayments");

            migrationBuilder.DropIndex(
                name: "IX_EventTickets_ExchangeRateId",
                table: "EventTickets");

            migrationBuilder.DropIndex(
                name: "IX_BalanceTransactions_CurrencyCode",
                table: "BalanceTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BalanceTransactions_ExchangeRateId",
                table: "BalanceTransactions");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "PlannerWithdrawalRequests");

            migrationBuilder.DropColumn(
                name: "ExchangeRateCapturedAtUtc",
                table: "PlannerWithdrawalRequests");

            migrationBuilder.DropColumn(
                name: "ExchangeRateId",
                table: "PlannerWithdrawalRequests");

            migrationBuilder.DropColumn(
                name: "ExchangeRateToIrr",
                table: "PlannerWithdrawalRequests");

            migrationBuilder.DropColumn(
                name: "ReportingAmountIrr",
                table: "PlannerWithdrawalRequests");

            migrationBuilder.DropColumn(
                name: "AccountHolderName",
                table: "PlannerBankAccounts");

            migrationBuilder.DropColumn(
                name: "AccountIdentifier",
                table: "PlannerBankAccounts");

            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "PlannerBankAccounts");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "PlannerBankAccounts");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "PlannerBankAccounts");

            migrationBuilder.DropColumn(
                name: "PayoutMethod",
                table: "PlannerBankAccounts");

            migrationBuilder.DropColumn(
                name: "PublicPaymentInstructions",
                table: "PlannerBankAccounts");

            migrationBuilder.DropColumn(
                name: "SwiftCode",
                table: "PlannerBankAccounts");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "OnlinePayments");

            migrationBuilder.DropColumn(
                name: "ExchangeRateCapturedAtUtc",
                table: "OnlinePayments");

            migrationBuilder.DropColumn(
                name: "ExchangeRateId",
                table: "OnlinePayments");

            migrationBuilder.DropColumn(
                name: "ExchangeRateToIrr",
                table: "OnlinePayments");

            migrationBuilder.DropColumn(
                name: "ReportingAmountIrr",
                table: "OnlinePayments");

            migrationBuilder.DropColumn(
                name: "ExchangeRateCapturedAtUtc",
                table: "EventTickets");

            migrationBuilder.DropColumn(
                name: "ExchangeRateId",
                table: "EventTickets");

            migrationBuilder.DropColumn(
                name: "ExchangeRateToIrr",
                table: "EventTickets");

            migrationBuilder.DropColumn(
                name: "ReportingOriginalPriceIrr",
                table: "EventTickets");

            migrationBuilder.DropColumn(
                name: "ReportingPriceIrr",
                table: "EventTickets");

            migrationBuilder.DropColumn(
                name: "SettlementCurrencyCode",
                table: "EventPlannerProfiles");

            migrationBuilder.DropColumn(
                name: "SettlementCurrencyLockReason",
                table: "EventPlannerProfiles");

            migrationBuilder.DropColumn(
                name: "SettlementCurrencyLockedAtUtc",
                table: "EventPlannerProfiles");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "BalanceTransactions");

            migrationBuilder.DropColumn(
                name: "ExchangeRateCapturedAtUtc",
                table: "BalanceTransactions");

            migrationBuilder.DropColumn(
                name: "ExchangeRateId",
                table: "BalanceTransactions");

            migrationBuilder.DropColumn(
                name: "ExchangeRateToIrr",
                table: "BalanceTransactions");

            migrationBuilder.DropColumn(
                name: "ReportingAmountIrr",
                table: "BalanceTransactions");

            migrationBuilder.DropColumn(
                name: "ReportingCurrencyCode",
                table: "BalanceTransactions");

            migrationBuilder.DropColumn(
                name: "ReportingCurrencyCode",
                table: "BalanceAccounts");

            migrationBuilder.AlterColumn<string>(
                name: "Iban",
                table: "PlannerBankAccounts",
                type: "nvarchar(34)",
                maxLength: 34,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(34)",
                oldMaxLength: 34,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CardNumber",
                table: "PlannerBankAccounts",
                type: "nvarchar(19)",
                maxLength: 19,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(19)",
                oldMaxLength: 19,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BankName",
                table: "PlannerBankAccounts",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlannerBankAccounts_Iban",
                table: "PlannerBankAccounts",
                column: "Iban",
                unique: true);
        }
    }
}
