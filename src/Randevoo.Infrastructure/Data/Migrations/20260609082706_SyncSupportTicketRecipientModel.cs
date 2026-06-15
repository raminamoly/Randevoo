using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SyncSupportTicketRecipientModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "DatingEvents",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "IRR");

            migrationBuilder.Sql(
                """
                UPDATE DatingEvents
                SET CurrencyCode = COALESCE(NULLIF(MaleTicketCurrencyCode, ''), 'IRR')
                """);

            migrationBuilder.AddColumn<int>(
                name: "DecimalPlaces",
                table: "Currencies",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.CreateTable(
                name: "ManualPaymentReceipts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatingEventId = table.Column<long>(type: "bigint", nullable: false),
                    ParticipantUserId = table.Column<long>(type: "bigint", nullable: false),
                    PlannerUserId = table.Column<long>(type: "bigint", nullable: false),
                    EventTicketId = table.Column<long>(type: "bigint", nullable: true),
                    EventDiscountCodeId = table.Column<long>(type: "bigint", nullable: true),
                    DiscountCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PaymentCollectionMethod = table.Column<int>(type: "int", nullable: false),
                    DestinationType = table.Column<int>(type: "int", nullable: false),
                    OriginalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "IRR"),
                    ReportingCurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "IRR"),
                    ReportingAmountIrr = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ExchangeRateToIrr = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false, defaultValue: 1m),
                    ExchangeRateCapturedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ExchangeRateId = table.Column<long>(type: "bigint", nullable: true),
                    UploadedFilePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TrackingNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    PayerNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubmittedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ReviewedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManualPaymentReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManualPaymentReceipts_CurrencyExchangeRates_ExchangeRateId",
                        column: x => x.ExchangeRateId,
                        principalTable: "CurrencyExchangeRates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManualPaymentReceipts_DatingEvents_DatingEventId",
                        column: x => x.DatingEventId,
                        principalTable: "DatingEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManualPaymentReceipts_EventDiscountCodes_EventDiscountCodeId",
                        column: x => x.EventDiscountCodeId,
                        principalTable: "EventDiscountCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManualPaymentReceipts_EventTickets_EventTicketId",
                        column: x => x.EventTicketId,
                        principalTable: "EventTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManualPaymentReceipts_Users_ParticipantUserId",
                        column: x => x.ParticipantUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManualPaymentReceipts_Users_PlannerUserId",
                        column: x => x.PlannerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManualPaymentReceipts_Users_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 1L,
                column: "DecimalPlaces",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 2L,
                column: "DecimalPlaces",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 3L,
                column: "DecimalPlaces",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 4L,
                column: "DecimalPlaces",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 5L,
                column: "DecimalPlaces",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 6L,
                column: "DecimalPlaces",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 7L,
                column: "DecimalPlaces",
                value: 2);

            migrationBuilder.CreateIndex(
                name: "IX_DatingEvents_CurrencyCode",
                table: "DatingEvents",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_ManualPaymentReceipts_CurrencyCode",
                table: "ManualPaymentReceipts",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_ManualPaymentReceipts_DatingEventId",
                table: "ManualPaymentReceipts",
                column: "DatingEventId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualPaymentReceipts_DestinationType_Status_SubmittedAtUtc",
                table: "ManualPaymentReceipts",
                columns: new[] { "DestinationType", "Status", "SubmittedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ManualPaymentReceipts_EventDiscountCodeId",
                table: "ManualPaymentReceipts",
                column: "EventDiscountCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualPaymentReceipts_EventTicketId",
                table: "ManualPaymentReceipts",
                column: "EventTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualPaymentReceipts_ExchangeRateId",
                table: "ManualPaymentReceipts",
                column: "ExchangeRateId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualPaymentReceipts_ParticipantUserId",
                table: "ManualPaymentReceipts",
                column: "ParticipantUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualPaymentReceipts_PlannerUserId",
                table: "ManualPaymentReceipts",
                column: "PlannerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualPaymentReceipts_ReviewedByUserId",
                table: "ManualPaymentReceipts",
                column: "ReviewedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManualPaymentReceipts");

            migrationBuilder.DropIndex(
                name: "IX_DatingEvents_CurrencyCode",
                table: "DatingEvents");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "DatingEvents");

            migrationBuilder.DropColumn(
                name: "DecimalPlaces",
                table: "Currencies");
        }
    }
}
