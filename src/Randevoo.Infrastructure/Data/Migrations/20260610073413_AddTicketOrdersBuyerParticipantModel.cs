using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketOrdersBuyerParticipantModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TicketOrderId",
                table: "OnlinePayments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TicketOrderId",
                table: "ManualPaymentReceipts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TicketOrderId",
                table: "EventTickets",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TicketOrderId",
                table: "BalanceTransactions",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TicketOrders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatingEventId = table.Column<long>(type: "bigint", nullable: false),
                    BuyerUserId = table.Column<long>(type: "bigint", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "IRR"),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PlatformCommissionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OrganizerIncomeAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentCollectionMethod = table.Column<int>(type: "int", nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    OrderStatus = table.Column<int>(type: "int", nullable: false),
                    EventDiscountCodeId = table.Column<long>(type: "bigint", nullable: true),
                    DiscountCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReportingCurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "IRR"),
                    ReportingGrossAmountIrr = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReportingDiscountAmountIrr = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReportingNetAmountIrr = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReportingPlatformCommissionIrr = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReportingOrganizerIncomeIrr = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ExchangeRateToIrr = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false, defaultValue: 1m),
                    ExchangeRateCapturedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ExchangeRateId = table.Column<long>(type: "bigint", nullable: true),
                    PaidAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketOrders_CurrencyExchangeRates_ExchangeRateId",
                        column: x => x.ExchangeRateId,
                        principalTable: "CurrencyExchangeRates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketOrders_DatingEvents_DatingEventId",
                        column: x => x.DatingEventId,
                        principalTable: "DatingEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketOrders_EventDiscountCodes_EventDiscountCodeId",
                        column: x => x.EventDiscountCodeId,
                        principalTable: "EventDiscountCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketOrders_Users_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketOrders_Users_BuyerUserId",
                        column: x => x.BuyerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql("""
                CREATE TABLE #TicketOrderBackfill
                (
                    TicketId BIGINT NOT NULL PRIMARY KEY,
                    TicketOrderId BIGINT NOT NULL
                );

                MERGE TicketOrders AS target
                USING
                (
                    SELECT
                        t.Id AS TicketId,
                        t.DatingEventId,
                        t.UserId AS BuyerUserId,
                        t.CurrencyCode,
                        t.OriginalPrice AS GrossAmount,
                        t.DiscountAmount,
                        t.Price AS NetAmount,
                        CAST(ROUND(t.Price * e.EventPlannerCommissionPercent / 100.0, 2) AS DECIMAL(18, 2)) AS PlatformCommissionAmount,
                        CAST(t.Price - ROUND(t.Price * e.EventPlannerCommissionPercent / 100.0, 2) AS DECIMAL(18, 2)) AS OrganizerIncomeAmount,
                        e.PaymentCollectionMethod,
                        CASE WHEN t.IsRefunded = 1 THEN 3 ELSE 1 END AS PaymentStatus,
                        CASE WHEN t.IsRefunded = 1 THEN 3 ELSE 1 END AS OrderStatus,
                        t.EventDiscountCodeId,
                        t.DiscountCode,
                        N'IRR' AS ReportingCurrencyCode,
                        t.ReportingOriginalPriceIrr AS ReportingGrossAmountIrr,
                        CAST(ROUND(t.DiscountAmount * t.ExchangeRateToIrr, 0) AS DECIMAL(18, 2)) AS ReportingDiscountAmountIrr,
                        t.ReportingPriceIrr AS ReportingNetAmountIrr,
                        CAST(ROUND((t.Price * e.EventPlannerCommissionPercent / 100.0) * t.ExchangeRateToIrr, 0) AS DECIMAL(18, 2)) AS ReportingPlatformCommissionIrr,
                        CAST(t.ReportingPriceIrr - ROUND((t.Price * e.EventPlannerCommissionPercent / 100.0) * t.ExchangeRateToIrr, 0) AS DECIMAL(18, 2)) AS ReportingOrganizerIncomeIrr,
                        t.ExchangeRateToIrr,
                        t.ExchangeRateCapturedAtUtc,
                        t.ExchangeRateId,
                        t.CreatedAt AS PaidAtUtc,
                        t.CreatedAt AS ApprovedAtUtc,
                        NULL AS ApprovedByUserId,
                        NULL AS Notes,
                        t.CreatedAt,
                        t.UpdatedAt,
                        t.IsDeleted,
                        t.DeletedAt
                    FROM EventTickets t
                    INNER JOIN DatingEvents e ON e.Id = t.DatingEventId
                    WHERE t.TicketOrderId IS NULL
                ) AS src
                ON 1 = 0
                WHEN NOT MATCHED THEN
                    INSERT
                    (
                        DatingEventId,
                        BuyerUserId,
                        CurrencyCode,
                        GrossAmount,
                        DiscountAmount,
                        NetAmount,
                        PlatformCommissionAmount,
                        OrganizerIncomeAmount,
                        PaymentCollectionMethod,
                        PaymentStatus,
                        OrderStatus,
                        EventDiscountCodeId,
                        DiscountCode,
                        ReportingCurrencyCode,
                        ReportingGrossAmountIrr,
                        ReportingDiscountAmountIrr,
                        ReportingNetAmountIrr,
                        ReportingPlatformCommissionIrr,
                        ReportingOrganizerIncomeIrr,
                        ExchangeRateToIrr,
                        ExchangeRateCapturedAtUtc,
                        ExchangeRateId,
                        PaidAtUtc,
                        ApprovedAtUtc,
                        ApprovedByUserId,
                        Notes,
                        CreatedAt,
                        UpdatedAt,
                        IsDeleted,
                        DeletedAt
                    )
                    VALUES
                    (
                        src.DatingEventId,
                        src.BuyerUserId,
                        src.CurrencyCode,
                        src.GrossAmount,
                        src.DiscountAmount,
                        src.NetAmount,
                        src.PlatformCommissionAmount,
                        src.OrganizerIncomeAmount,
                        src.PaymentCollectionMethod,
                        src.PaymentStatus,
                        src.OrderStatus,
                        src.EventDiscountCodeId,
                        src.DiscountCode,
                        src.ReportingCurrencyCode,
                        src.ReportingGrossAmountIrr,
                        src.ReportingDiscountAmountIrr,
                        src.ReportingNetAmountIrr,
                        src.ReportingPlatformCommissionIrr,
                        src.ReportingOrganizerIncomeIrr,
                        src.ExchangeRateToIrr,
                        src.ExchangeRateCapturedAtUtc,
                        src.ExchangeRateId,
                        src.PaidAtUtc,
                        src.ApprovedAtUtc,
                        src.ApprovedByUserId,
                        src.Notes,
                        src.CreatedAt,
                        src.UpdatedAt,
                        src.IsDeleted,
                        src.DeletedAt
                    )
                OUTPUT src.TicketId, inserted.Id INTO #TicketOrderBackfill;

                UPDATE t
                SET TicketOrderId = backfill.TicketOrderId
                FROM EventTickets t
                INNER JOIN #TicketOrderBackfill backfill ON backfill.TicketId = t.Id;

                UPDATE payment
                SET TicketOrderId = ticket.TicketOrderId
                FROM OnlinePayments payment
                INNER JOIN EventTickets ticket ON ticket.Id = payment.EventTicketId
                WHERE payment.TicketOrderId IS NULL
                  AND payment.EventTicketId IS NOT NULL;

                UPDATE receipt
                SET TicketOrderId = ticket.TicketOrderId
                FROM ManualPaymentReceipts receipt
                INNER JOIN EventTickets ticket ON ticket.Id = receipt.EventTicketId
                WHERE receipt.TicketOrderId IS NULL
                  AND receipt.EventTicketId IS NOT NULL;

                UPDATE transactionItem
                SET TicketOrderId = ticket.TicketOrderId
                FROM BalanceTransactions transactionItem
                INNER JOIN EventTickets ticket
                    ON transactionItem.ReferenceType = N'EventTicket'
                    AND transactionItem.ReferenceId = ticket.Id
                WHERE transactionItem.TicketOrderId IS NULL;

                UPDATE transactionItem
                SET TicketOrderId = ticket.TicketOrderId
                FROM BalanceTransactions transactionItem
                INNER JOIN EventTickets ticket
                    ON transactionItem.DatingEventId = ticket.DatingEventId
                    AND transactionItem.UserId = ticket.UserId
                WHERE transactionItem.TicketOrderId IS NULL
                  AND transactionItem.Type IN (1, 5)
                  AND ticket.TicketOrderId IS NOT NULL;

                DROP TABLE #TicketOrderBackfill;
                """);

            migrationBuilder.AlterColumn<long>(
                name: "TicketOrderId",
                table: "EventTickets",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OnlinePayments_TicketOrderId",
                table: "OnlinePayments",
                column: "TicketOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualPaymentReceipts_TicketOrderId",
                table: "ManualPaymentReceipts",
                column: "TicketOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTickets_TicketOrderId",
                table: "EventTickets",
                column: "TicketOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceTransactions_TicketOrderId",
                table: "BalanceTransactions",
                column: "TicketOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketOrders_ApprovedByUserId",
                table: "TicketOrders",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketOrders_BuyerUserId",
                table: "TicketOrders",
                column: "BuyerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketOrders_CurrencyCode",
                table: "TicketOrders",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_TicketOrders_DatingEventId",
                table: "TicketOrders",
                column: "DatingEventId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketOrders_EventDiscountCodeId",
                table: "TicketOrders",
                column: "EventDiscountCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketOrders_ExchangeRateId",
                table: "TicketOrders",
                column: "ExchangeRateId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketOrders_OrderStatus_CreatedAt",
                table: "TicketOrders",
                columns: new[] { "OrderStatus", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TicketOrders_PaymentStatus_CreatedAt",
                table: "TicketOrders",
                columns: new[] { "PaymentStatus", "CreatedAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_BalanceTransactions_TicketOrders_TicketOrderId",
                table: "BalanceTransactions",
                column: "TicketOrderId",
                principalTable: "TicketOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventTickets_TicketOrders_TicketOrderId",
                table: "EventTickets",
                column: "TicketOrderId",
                principalTable: "TicketOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ManualPaymentReceipts_TicketOrders_TicketOrderId",
                table: "ManualPaymentReceipts",
                column: "TicketOrderId",
                principalTable: "TicketOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OnlinePayments_TicketOrders_TicketOrderId",
                table: "OnlinePayments",
                column: "TicketOrderId",
                principalTable: "TicketOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BalanceTransactions_TicketOrders_TicketOrderId",
                table: "BalanceTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_EventTickets_TicketOrders_TicketOrderId",
                table: "EventTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_ManualPaymentReceipts_TicketOrders_TicketOrderId",
                table: "ManualPaymentReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_OnlinePayments_TicketOrders_TicketOrderId",
                table: "OnlinePayments");

            migrationBuilder.DropTable(
                name: "TicketOrders");

            migrationBuilder.DropIndex(
                name: "IX_OnlinePayments_TicketOrderId",
                table: "OnlinePayments");

            migrationBuilder.DropIndex(
                name: "IX_ManualPaymentReceipts_TicketOrderId",
                table: "ManualPaymentReceipts");

            migrationBuilder.DropIndex(
                name: "IX_EventTickets_TicketOrderId",
                table: "EventTickets");

            migrationBuilder.DropIndex(
                name: "IX_BalanceTransactions_TicketOrderId",
                table: "BalanceTransactions");

            migrationBuilder.DropColumn(
                name: "TicketOrderId",
                table: "OnlinePayments");

            migrationBuilder.DropColumn(
                name: "TicketOrderId",
                table: "ManualPaymentReceipts");

            migrationBuilder.DropColumn(
                name: "TicketOrderId",
                table: "EventTickets");

            migrationBuilder.DropColumn(
                name: "TicketOrderId",
                table: "BalanceTransactions");
        }
    }
}
