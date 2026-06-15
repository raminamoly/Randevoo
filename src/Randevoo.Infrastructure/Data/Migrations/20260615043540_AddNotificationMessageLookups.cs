using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationMessageLookups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationMessageTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    DisplayNameFa = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    DescriptionFa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RequiresApproval = table.Column<bool>(type: "bit", nullable: false),
                    SupportsSms = table.Column<bool>(type: "bit", nullable: false),
                    AllowedSenderRoles = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AllowedTargets = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DefaultPriority = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationMessageTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationPriorities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    DisplayNameFa = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    DescriptionFa = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationPriorities", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "NotificationMessageTypes",
                columns: new[] { "Id", "AllowedSenderRoles", "AllowedTargets", "Code", "CreatedAt", "DefaultPriority", "DeletedAt", "DescriptionFa", "DisplayNameFa", "DisplayOrder", "IsActive", "IsDeleted", "RequiresApproval", "SupportsSms", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, "Admin,PlatformSupportTeam", "User,EventParticipants,EventBuyers,Planners", "System", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 0, null, "پیام‌های خودکار سیستم برای اطلاع‌رسانی داخلی.", "پیام سیستمی", 10, true, false, false, false, 0, null },
                    { 2L, "Admin,PlatformSupportTeam", "User,Planners", "AdminToPlanner", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 0, null, "پیام مدیریتی یا عملیاتی برای برگزارکننده‌ها.", "پیام مدیر به برگزارکننده", 20, true, false, false, false, 1, null },
                    { 3L, "EventPlanner", "EventParticipants,EventBuyers,User", "PlannerToParticipant", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, "پیام برگزارکننده فقط برای شرکت‌کنندگان یا خریداران رویدادهای خودش.", "پیام برگزارکننده به شرکت‌کننده", 30, true, false, true, true, 2, null },
                    { 4L, "Admin,PlatformSupportTeam", "User,EventParticipants,EventBuyers", "AdminToUser", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 0, null, "پیام مستقیم مدیر یا پشتیبان به یک کاربر یا گروه مجاز.", "پیام مدیر به کاربر", 40, true, false, false, false, 3, null },
                    { 5L, "Admin,PlatformSupportTeam,EventPlanner", "EventParticipants,EventBuyers", "EventUpdate", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, "اطلاع‌رسانی تغییرات زمان، مکان یا جزئیات رویداد.", "اطلاع‌رسانی رویداد", 50, true, false, true, true, 4, null },
                    { 6L, "Admin,PlatformSupportTeam", "User,EventParticipants,EventBuyers,Planners", "Finance", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, "اطلاع‌رسانی مالی، رسید، تسویه یا وضعیت پرداخت.", "پیام مالی", 60, true, false, false, false, 5, null },
                    { 7L, "Admin,PlatformSupportTeam", "User,EventParticipants,EventBuyers", "Refund", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, "اطلاع‌رسانی مربوط به درخواست یا نتیجه بازگشت وجه.", "بازگشت وجه", 70, true, false, false, false, 6, null }
                });

            migrationBuilder.InsertData(
                table: "NotificationPriorities",
                columns: new[] { "Id", "Code", "CreatedAt", "DeletedAt", "DescriptionFa", "DisplayNameFa", "DisplayOrder", "IsActive", "IsDeleted", "Priority", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, "Normal", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), null, "پیام اطلاع‌رسانی معمولی.", "عادی", 10, true, false, 0, null },
                    { 2L, "Important", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), null, "پیامی که بهتر است کاربر زودتر ببیند.", "مهم", 20, true, false, 1, null },
                    { 3L, "Critical", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), null, "پیام حساس درباره تغییر مهم، مالی یا لغو.", "فوری", 30, true, false, 2, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationMessageTypes_Code",
                table: "NotificationMessageTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationMessageTypes_IsActive_DisplayOrder",
                table: "NotificationMessageTypes",
                columns: new[] { "IsActive", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationPriorities_Code",
                table: "NotificationPriorities",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationMessageTypes");

            migrationBuilder.DropTable(
                name: "NotificationPriorities");
        }
    }
}
