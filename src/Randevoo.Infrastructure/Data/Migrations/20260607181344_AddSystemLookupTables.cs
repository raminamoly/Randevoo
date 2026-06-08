using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemLookupTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BalanceTransactionTypes",
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
                    table.PrimaryKey("PK_BalanceTransactionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscountTypes",
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
                    table.PrimaryKey("PK_DiscountTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReviewStatuses",
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
                    table.PrimaryKey("PK_ReviewStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
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
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "BalanceTransactionTypes",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "DisplayNameFa", "DisplayOrder", "IsActive", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, "اصلاح مدیر", 1, true, false, "AdminAdjustment", null },
                    { 2L, new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, "خرید بلیت", 2, true, false, "TicketPurchase", null },
                    { 3L, new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, "بازگشت بلیت", 3, true, false, "TicketRefund", null },
                    { 4L, new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, "درآمد برگزارکننده", 4, true, false, "EventPlannerIncome", null },
                    { 5L, new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, "کمیسیون پلتفرم", 5, true, false, "PlatformCommission", null },
                    { 6L, new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, "بازگشت حذف اضطراری", 6, true, false, "EmergencyRemovalRefund", null },
                    { 7L, new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, "تسویه برگزارکننده", 7, true, false, "PlannerWithdrawalPayout", null },
                    { 8L, new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, "برگشت درآمد برگزارکننده", 8, true, false, "EventPlannerIncomeReversal", null }
                });

            migrationBuilder.InsertData(
                table: "DiscountTypes",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "DisplayNameFa", "DisplayOrder", "IsActive", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, "مبلغ ثابت", 1, true, false, "FixedAmount", null },
                    { 2L, new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, "درصدی", 2, true, false, "Percentage", null }
                });

            migrationBuilder.InsertData(
                table: "ReviewStatuses",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "DisplayNameFa", "DisplayOrder", "IsActive", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, "ارسال نشده", 1, true, false, "NotSubmitted", null },
                    { 2L, new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, "در انتظار بررسی", 2, true, false, "PendingReview", null },
                    { 3L, new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, "تایید شده توسط مدیر", 3, true, false, "Approved", null },
                    { 4L, new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, "رد شده توسط مدیر", 4, true, false, "Rejected", null }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "DisplayNameFa", "DisplayOrder", "IsActive", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, "شرکت‌کننده", 1, true, false, "EndUser", null },
                    { 2L, new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, "برگزارکننده", 2, true, false, "EventPlanner", null },
                    { 3L, new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, "مدیر", 3, true, false, "Admin", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BalanceTransactionTypes_Name",
                table: "BalanceTransactionTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscountTypes_Name",
                table: "DiscountTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewStatuses_Name",
                table: "ReviewStatuses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_Name",
                table: "UserRoles",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BalanceTransactionTypes");

            migrationBuilder.DropTable(
                name: "DiscountTypes");

            migrationBuilder.DropTable(
                name: "ReviewStatuses");

            migrationBuilder.DropTable(
                name: "UserRoles");
        }
    }
}
