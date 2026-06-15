using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRefundsAndNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ApprovalStatus = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    DatingEventId = table.Column<long>(type: "bigint", nullable: true),
                    ReferenceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReferenceId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    ReviewedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_DatingEvents_DatingEventId",
                        column: x => x.DatingEventId,
                        principalTable: "DatingEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTemplates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    TitleTemplate = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    BodyTemplate = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    RequiresApproval = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketRefundRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventTicketId = table.Column<long>(type: "bigint", nullable: false),
                    TicketOrderId = table.Column<long>(type: "bigint", nullable: false),
                    DatingEventId = table.Column<long>(type: "bigint", nullable: false),
                    BuyerUserId = table.Column<long>(type: "bigint", nullable: false),
                    ParticipantUserId = table.Column<long>(type: "bigint", nullable: false),
                    RequestedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ApprovedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "IRR"),
                    ReportingRequestedAmountIrr = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReportingApprovedAmountIrr = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ExchangeRateToIrr = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false, defaultValue: 1m),
                    ExchangeRateCapturedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ExchangeRateId = table.Column<long>(type: "bigint", nullable: true),
                    RequestReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    RequestedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ReviewedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    WalletCreditTransactionId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketRefundRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketRefundRequests_BalanceTransactions_WalletCreditTransactionId",
                        column: x => x.WalletCreditTransactionId,
                        principalTable: "BalanceTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketRefundRequests_CurrencyExchangeRates_ExchangeRateId",
                        column: x => x.ExchangeRateId,
                        principalTable: "CurrencyExchangeRates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketRefundRequests_DatingEvents_DatingEventId",
                        column: x => x.DatingEventId,
                        principalTable: "DatingEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketRefundRequests_EventTickets_EventTicketId",
                        column: x => x.EventTicketId,
                        principalTable: "EventTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketRefundRequests_TicketOrders_TicketOrderId",
                        column: x => x.TicketOrderId,
                        principalTable: "TicketOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketRefundRequests_Users_BuyerUserId",
                        column: x => x.BuyerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketRefundRequests_Users_ParticipantUserId",
                        column: x => x.ParticipantUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketRefundRequests_Users_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketRefundRequests_Users_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationRecipients",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotificationId = table.Column<long>(type: "bigint", nullable: false),
                    RecipientUserId = table.Column<long>(type: "bigint", nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DeliveredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReadAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationRecipients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationRecipients_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationRecipients_Users_RecipientUserId",
                        column: x => x.RecipientUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "NotificationTemplates",
                columns: new[] { "Id", "BodyTemplate", "Code", "CreatedAt", "DeletedAt", "IsActive", "IsDeleted", "Priority", "RequiresApproval", "TitleTemplate", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, "رویداد {EventTitle} لغو شد و مبلغ پرداختی به کیف پول شما اضافه می‌شود.", "event-cancelled", new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, 2, false, "لغو رویداد", 4, null },
                    { 2L, "درخواست بازگشت وجه شما برای {EventTitle} تایید و مبلغ به کیف پول اضافه شد.", "refund-approved", new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, 1, false, "بازگشت وجه تایید شد", 6, null },
                    { 3L, "رسید پرداخت شما برای رویداد لغوشده {EventTitle} تایید شد و مبلغ به کیف پول اضافه شد.", "manual-receipt-wallet-credit", new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, 1, false, "رسید پرداخت به کیف پول منتقل شد", 5, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRecipients_NotificationId_RecipientUserId_Channel",
                table: "NotificationRecipients",
                columns: new[] { "NotificationId", "RecipientUserId", "Channel" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRecipients_ReadAtUtc",
                table: "NotificationRecipients",
                column: "ReadAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRecipients_RecipientUserId_Status_CreatedAt",
                table: "NotificationRecipients",
                columns: new[] { "RecipientUserId", "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ApprovalStatus_CreatedAt",
                table: "Notifications",
                columns: new[] { "ApprovalStatus", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreatedByUserId",
                table: "Notifications",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_DatingEventId",
                table: "Notifications",
                column: "DatingEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ReferenceType_ReferenceId",
                table: "Notifications",
                columns: new[] { "ReferenceType", "ReferenceId" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ReviewedByUserId",
                table: "Notifications",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Type_CreatedAt",
                table: "Notifications",
                columns: new[] { "Type", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplates_Code",
                table: "NotificationTemplates",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplates_Type_IsActive",
                table: "NotificationTemplates",
                columns: new[] { "Type", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_TicketRefundRequests_BuyerUserId",
                table: "TicketRefundRequests",
                column: "BuyerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketRefundRequests_DatingEventId",
                table: "TicketRefundRequests",
                column: "DatingEventId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketRefundRequests_EventTicketId",
                table: "TicketRefundRequests",
                column: "EventTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketRefundRequests_ExchangeRateId",
                table: "TicketRefundRequests",
                column: "ExchangeRateId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketRefundRequests_ParticipantUserId",
                table: "TicketRefundRequests",
                column: "ParticipantUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketRefundRequests_RequestedByUserId",
                table: "TicketRefundRequests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketRefundRequests_ReviewedByUserId",
                table: "TicketRefundRequests",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketRefundRequests_Status_RequestedAtUtc",
                table: "TicketRefundRequests",
                columns: new[] { "Status", "RequestedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_TicketRefundRequests_TicketOrderId",
                table: "TicketRefundRequests",
                column: "TicketOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketRefundRequests_WalletCreditTransactionId",
                table: "TicketRefundRequests",
                column: "WalletCreditTransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationRecipients");

            migrationBuilder.DropTable(
                name: "NotificationTemplates");

            migrationBuilder.DropTable(
                name: "TicketRefundRequests");

            migrationBuilder.DropTable(
                name: "Notifications");
        }
    }
}
