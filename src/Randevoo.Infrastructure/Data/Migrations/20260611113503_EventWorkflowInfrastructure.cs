using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class EventWorkflowInfrastructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminReviewNote",
                table: "DatingEvents",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "DatingEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAtUtc",
                table: "DatingEvents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ApprovedByUserId",
                table: "DatingEvents",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "DatingEvents",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAtUtc",
                table: "DatingEvents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CancelledByUserId",
                table: "DatingEvents",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAtUtc",
                table: "DatingEvents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LifecycleStatus",
                table: "DatingEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaleStatus",
                table: "DatingEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                UPDATE DatingEvents
                SET
                    ApprovalStatus = CASE
                        WHEN ReviewStatus = 1 THEN 1
                        WHEN ReviewStatus = 2 THEN 2
                        ELSE 0
                    END,
                    SaleStatus = CASE WHEN IsOpenForSell = 1 THEN 1 ELSE 0 END,
                    LifecycleStatus = CASE
                        WHEN IsCancelled = 1 THEN 1
                        WHEN DateTimeEnd <= SYSUTCDATETIME() THEN 2
                        ELSE 0
                    END,
                    CompletedAtUtc = CASE
                        WHEN IsCancelled = 0 AND DateTimeEnd <= SYSUTCDATETIME() THEN DateTimeEnd
                        ELSE CompletedAtUtc
                    END,
                    ReviewStatus = CASE WHEN ReviewStatus = 3 THEN 0 ELSE ReviewStatus END
                """);

            migrationBuilder.CreateTable(
                name: "EventApprovalStatuses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayNameFa = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventApprovalStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventCancellationRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatingEventId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    RequestedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ReviewNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventCancellationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventCancellationRequests_DatingEvents_DatingEventId",
                        column: x => x.DatingEventId,
                        principalTable: "DatingEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventCancellationRequests_Users_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventCancellationRequests_Users_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventChangeRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatingEventId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    RequestedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    BeforeJson = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    AfterJson = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    ReviewNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventChangeRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventChangeRequests_DatingEvents_DatingEventId",
                        column: x => x.DatingEventId,
                        principalTable: "DatingEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventChangeRequests_Users_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventChangeRequests_Users_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventLifecycleStatuses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayNameFa = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventLifecycleStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventRequestStatuses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayNameFa = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventRequestStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventSaleStatuses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayNameFa = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSaleStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventSettlementRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatingEventId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    RequestedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidTicketCount = table.Column<int>(type: "int", nullable: false),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PlatformCommissionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OrganizerIncomeAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReportingOrganizerIncomeIrr = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OrganizerCreditTransactionId = table.Column<long>(type: "bigint", nullable: true),
                    RequestNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSettlementRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventSettlementRequests_BalanceTransactions_OrganizerCreditTransactionId",
                        column: x => x.OrganizerCreditTransactionId,
                        principalTable: "BalanceTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventSettlementRequests_DatingEvents_DatingEventId",
                        column: x => x.DatingEventId,
                        principalTable: "DatingEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventSettlementRequests_Users_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventSettlementRequests_Users_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventWorkflowActionTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    DisplayNameFa = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventWorkflowActionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventWorkflowLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatingEventId = table.Column<long>(type: "bigint", nullable: false),
                    ActionType = table.Column<int>(type: "int", nullable: false),
                    FromApprovalStatus = table.Column<int>(type: "int", nullable: true),
                    ToApprovalStatus = table.Column<int>(type: "int", nullable: true),
                    FromSaleStatus = table.Column<int>(type: "int", nullable: true),
                    ToSaleStatus = table.Column<int>(type: "int", nullable: true),
                    FromLifecycleStatus = table.Column<int>(type: "int", nullable: true),
                    ToLifecycleStatus = table.Column<int>(type: "int", nullable: true),
                    ActorUserId = table.Column<long>(type: "bigint", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    BeforeJson = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    AfterJson = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    MetadataJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventWorkflowLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventWorkflowLogs_DatingEvents_DatingEventId",
                        column: x => x.DatingEventId,
                        principalTable: "DatingEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventWorkflowLogs_Users_ActorUserId",
                        column: x => x.ActorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "BalanceTransactionTypes",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "DisplayNameFa", "DisplayOrder", "IsActive", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 9L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "بستانکاری تسویه رویداد", 9, true, false, "EventSettlementCredit", null },
                    { 10L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "برگشت بستانکاری رویداد", 10, true, false, "EventSettlementReversal", null },
                    { 11L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "شناسایی کمیسیون پلتفرم", 11, true, false, "PlatformCommissionRecognized", null }
                });

            migrationBuilder.InsertData(
                table: "EventApprovalStatuses",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "DisplayNameFa", "DisplayOrder", "IsActive", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "پیش‌نویس", 1, true, false, "Draft", null },
                    { 2L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "در انتظار بررسی مدیر", 2, true, false, "PendingReview", null },
                    { 3L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "تایید شده", 3, true, false, "Approved", null }
                });

            migrationBuilder.InsertData(
                table: "EventLifecycleStatuses",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "DisplayNameFa", "DisplayOrder", "IsActive", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "فعال", 1, true, false, "Active", null },
                    { 2L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "لغو شده", 2, true, false, "Cancelled", null },
                    { 3L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "تمام شده", 3, true, false, "Completed", null }
                });

            migrationBuilder.InsertData(
                table: "EventRequestStatuses",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "DisplayNameFa", "DisplayOrder", "IsActive", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "در انتظار بررسی", 1, true, false, "Pending", null },
                    { 2L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "تایید شده", 2, true, false, "Approved", null },
                    { 3L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "رد شده", 3, true, false, "Rejected", null },
                    { 4L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "لغو شده", 4, true, false, "Cancelled", null }
                });

            migrationBuilder.InsertData(
                table: "EventSaleStatuses",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "DisplayNameFa", "DisplayOrder", "IsActive", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "فروش بسته", 1, true, false, "Closed", null },
                    { 2L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "در حال فروش", 2, true, false, "Open", null }
                });

            migrationBuilder.InsertData(
                table: "EventWorkflowActionTypes",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "DisplayNameFa", "DisplayOrder", "IsActive", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "ذخیره پیش‌نویس", 1, true, false, "DraftSaved", null },
                    { 2L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "ارسال برای بررسی", 2, true, false, "SubmittedForReview", null },
                    { 3L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "تایید رویداد", 3, true, false, "Approved", null },
                    { 4L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "بازگشت برای اصلاح", 4, true, false, "ReturnedToDraft", null },
                    { 5L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "باز شدن فروش", 5, true, false, "SaleOpened", null },
                    { 6L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "بسته شدن فروش", 6, true, false, "SaleClosed", null },
                    { 7L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "درخواست تغییر", 7, true, false, "ChangeRequested", null },
                    { 8L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "تایید تغییر", 8, true, false, "ChangeApproved", null },
                    { 9L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "رد تغییر", 9, true, false, "ChangeRejected", null },
                    { 10L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "درخواست لغو", 10, true, false, "CancellationRequested", null },
                    { 11L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "لغو رویداد", 11, true, false, "Cancelled", null },
                    { 12L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "اتمام رویداد", 12, true, false, "Completed", null },
                    { 13L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "درخواست تسویه رویداد", 13, true, false, "SettlementRequested", null },
                    { 14L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "تایید تسویه رویداد", 14, true, false, "SettlementApproved", null },
                    { 15L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "بستانکاری برگزارکننده", 15, true, false, "OrganizerCredited", null },
                    { 16L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "درخواست برداشت", 16, true, false, "WithdrawalRequested", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatingEvents_ApprovalStatus_DateTimeStart",
                table: "DatingEvents",
                columns: new[] { "ApprovalStatus", "DateTimeStart" });

            migrationBuilder.CreateIndex(
                name: "IX_DatingEvents_ApprovedByUserId",
                table: "DatingEvents",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DatingEvents_CancelledByUserId",
                table: "DatingEvents",
                column: "CancelledByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DatingEvents_LifecycleStatus_SaleStatus_DateTimeEnd",
                table: "DatingEvents",
                columns: new[] { "LifecycleStatus", "SaleStatus", "DateTimeEnd" });

            migrationBuilder.CreateIndex(
                name: "IX_EventApprovalStatuses_Name",
                table: "EventApprovalStatuses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventCancellationRequests_DatingEventId_Status_RequestedAtUtc",
                table: "EventCancellationRequests",
                columns: new[] { "DatingEventId", "Status", "RequestedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_EventCancellationRequests_RequestedByUserId",
                table: "EventCancellationRequests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventCancellationRequests_ReviewedByUserId",
                table: "EventCancellationRequests",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventChangeRequests_DatingEventId_Status_RequestedAtUtc",
                table: "EventChangeRequests",
                columns: new[] { "DatingEventId", "Status", "RequestedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_EventChangeRequests_RequestedByUserId",
                table: "EventChangeRequests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventChangeRequests_ReviewedByUserId",
                table: "EventChangeRequests",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventLifecycleStatuses_Name",
                table: "EventLifecycleStatuses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventRequestStatuses_Name",
                table: "EventRequestStatuses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventSaleStatuses_Name",
                table: "EventSaleStatuses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventSettlementRequests_DatingEventId_Status_RequestedAtUtc",
                table: "EventSettlementRequests",
                columns: new[] { "DatingEventId", "Status", "RequestedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_EventSettlementRequests_OrganizerCreditTransactionId",
                table: "EventSettlementRequests",
                column: "OrganizerCreditTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSettlementRequests_RequestedByUserId",
                table: "EventSettlementRequests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSettlementRequests_ReviewedByUserId",
                table: "EventSettlementRequests",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventWorkflowActionTypes_Name",
                table: "EventWorkflowActionTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventWorkflowLogs_ActionType",
                table: "EventWorkflowLogs",
                column: "ActionType");

            migrationBuilder.CreateIndex(
                name: "IX_EventWorkflowLogs_ActorUserId",
                table: "EventWorkflowLogs",
                column: "ActorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventWorkflowLogs_DatingEventId_CreatedAt",
                table: "EventWorkflowLogs",
                columns: new[] { "DatingEventId", "CreatedAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_DatingEvents_Users_ApprovedByUserId",
                table: "DatingEvents",
                column: "ApprovedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DatingEvents_Users_CancelledByUserId",
                table: "DatingEvents",
                column: "CancelledByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatingEvents_Users_ApprovedByUserId",
                table: "DatingEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_DatingEvents_Users_CancelledByUserId",
                table: "DatingEvents");

            migrationBuilder.DropTable(
                name: "EventApprovalStatuses");

            migrationBuilder.DropTable(
                name: "EventCancellationRequests");

            migrationBuilder.DropTable(
                name: "EventChangeRequests");

            migrationBuilder.DropTable(
                name: "EventLifecycleStatuses");

            migrationBuilder.DropTable(
                name: "EventRequestStatuses");

            migrationBuilder.DropTable(
                name: "EventSaleStatuses");

            migrationBuilder.DropTable(
                name: "EventSettlementRequests");

            migrationBuilder.DropTable(
                name: "EventWorkflowActionTypes");

            migrationBuilder.DropTable(
                name: "EventWorkflowLogs");

            migrationBuilder.DropIndex(
                name: "IX_DatingEvents_ApprovalStatus_DateTimeStart",
                table: "DatingEvents");

            migrationBuilder.DropIndex(
                name: "IX_DatingEvents_ApprovedByUserId",
                table: "DatingEvents");

            migrationBuilder.DropIndex(
                name: "IX_DatingEvents_CancelledByUserId",
                table: "DatingEvents");

            migrationBuilder.DropIndex(
                name: "IX_DatingEvents_LifecycleStatus_SaleStatus_DateTimeEnd",
                table: "DatingEvents");

            migrationBuilder.DeleteData(
                table: "BalanceTransactionTypes",
                keyColumn: "Id",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "BalanceTransactionTypes",
                keyColumn: "Id",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "BalanceTransactionTypes",
                keyColumn: "Id",
                keyValue: 11L);

            migrationBuilder.DropColumn(
                name: "AdminReviewNote",
                table: "DatingEvents");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "DatingEvents");

            migrationBuilder.DropColumn(
                name: "ApprovedAtUtc",
                table: "DatingEvents");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "DatingEvents");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "DatingEvents");

            migrationBuilder.DropColumn(
                name: "CancelledAtUtc",
                table: "DatingEvents");

            migrationBuilder.DropColumn(
                name: "CancelledByUserId",
                table: "DatingEvents");

            migrationBuilder.DropColumn(
                name: "CompletedAtUtc",
                table: "DatingEvents");

            migrationBuilder.DropColumn(
                name: "LifecycleStatus",
                table: "DatingEvents");

            migrationBuilder.DropColumn(
                name: "SaleStatus",
                table: "DatingEvents");
        }
    }
}
