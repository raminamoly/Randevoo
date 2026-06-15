using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOperationPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PermissionActions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Entity = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleOperationPermissions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Allowed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleOperationPermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserOperationPermissionOverrides",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Allowed = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOperationPermissionOverrides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserOperationPermissionOverrides_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PermissionActions_Entity_Action",
                table: "PermissionActions",
                columns: new[] { "Entity", "Action" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleOperationPermissions_Entity_Action",
                table: "RoleOperationPermissions",
                columns: new[] { "Entity", "Action" });

            migrationBuilder.CreateIndex(
                name: "IX_RoleOperationPermissions_Role_Entity_Action",
                table: "RoleOperationPermissions",
                columns: new[] { "Role", "Entity", "Action" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserOperationPermissionOverrides_Entity_Action",
                table: "UserOperationPermissionOverrides",
                columns: new[] { "Entity", "Action" });

            migrationBuilder.CreateIndex(
                name: "IX_UserOperationPermissionOverrides_ExpiresAtUtc",
                table: "UserOperationPermissionOverrides",
                column: "ExpiresAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_UserOperationPermissionOverrides_UserId_Entity_Action",
                table: "UserOperationPermissionOverrides",
                columns: new[] { "UserId", "Entity", "Action" },
                unique: true);

            var createdAt = new DateTime(2026, 6, 10, 0, 0, 0, DateTimeKind.Utc);
            migrationBuilder.InsertData(
                table: "PermissionActions",
                columns: new[] { "Id", "Entity", "Action", "Label", "Description", "IsActive", "DisplayOrder", "CreatedAt", "IsDeleted" },
                columnTypes: new[] { "bigint", "nvarchar(80)", "nvarchar(80)", "nvarchar(120)", "nvarchar(500)", "bit", "int", "datetime2", "bit" },
                values: new object[,]
                {
                    { 1L, "participants", "list", "مشاهده فهرست", "دیدن فهرست شرکت‌کنندگان", true, 1, createdAt, false },
                    { 2L, "participants", "viewDetails", "مشاهده جزئیات", "باز کردن پروفایل شرکت‌کننده", true, 2, createdAt, false },
                    { 3L, "participants", "viewContactInfo", "مشاهده اطلاعات تماس", "نمایش شماره موبایل و اطلاعات تماس شرکت‌کننده", true, 3, createdAt, false },
                    { 4L, "participants", "editProfile", "ویرایش پروفایل", "ویرایش اطلاعات پروفایل شرکت‌کننده", true, 4, createdAt, false },
                    { 5L, "participants", "viewFinance", "مشاهده مالی", "مشاهده موجودی و پرداخت‌های شرکت‌کننده", true, 5, createdAt, false },
                    { 6L, "participants", "viewOrder", "مشاهده سفارش", "مشاهده تراکنش یا سفارش مرتبط با بلیت", true, 6, createdAt, false },
                    { 7L, "participants", "resendProfileLink", "ارسال لینک تکمیل پروفایل", "ارسال یا ارسال مجدد دعوت تکمیل پروفایل", true, 7, createdAt, false },
                    { 8L, "participants", "changeStatus", "تغییر وضعیت", "تغییر وضعیت شرکت‌کننده در رویداد", true, 8, createdAt, false },
                    { 9L, "participants", "replaceParticipant", "جایگزینی شرکت‌کننده", "جایگزین کردن شرکت‌کننده بلیت", true, 9, createdAt, false },
                    { 10L, "participants", "emergencyRefund", "بازگشت اضطراری", "حذف اضطراری شرکت‌کننده و بازگشت وجه", true, 10, createdAt, false },
                    { 11L, "participants", "export", "خروجی گرفتن", "دریافت خروجی از فهرست شرکت‌کنندگان", true, 11, createdAt, false },
                    { 12L, "events", "viewParticipants", "فهرست شرکت‌کنندگان رویداد", "ورود از رویداد به فهرست شرکت‌کنندگان", true, 1, createdAt, false },
                    { 13L, "orders", "view", "مشاهده سفارش", "مشاهده سفارش‌ها و تراکنش‌های مرتبط", true, 1, createdAt, false },
                    { 14L, "users", "manageOperationPermissions", "مدیریت دسترسی عملیات", "تغییر سطح دسترسی نقش‌ها و کاربران", true, 1, createdAt, false }
                });

            migrationBuilder.InsertData(
                table: "RoleOperationPermissions",
                columns: new[] { "Id", "Role", "Entity", "Action", "Allowed", "CreatedAt", "IsDeleted" },
                columnTypes: new[] { "bigint", "int", "nvarchar(80)", "nvarchar(80)", "bit", "datetime2", "bit" },
                values: new object[,]
                {
                    { 1L, 2, "participants", "list", true, createdAt, false },
                    { 2L, 2, "participants", "viewDetails", true, createdAt, false },
                    { 3L, 2, "participants", "viewContactInfo", true, createdAt, false },
                    { 4L, 2, "participants", "editProfile", true, createdAt, false },
                    { 5L, 2, "participants", "viewFinance", true, createdAt, false },
                    { 6L, 2, "participants", "viewOrder", true, createdAt, false },
                    { 7L, 2, "participants", "resendProfileLink", true, createdAt, false },
                    { 8L, 2, "participants", "changeStatus", true, createdAt, false },
                    { 9L, 2, "participants", "replaceParticipant", true, createdAt, false },
                    { 10L, 2, "participants", "emergencyRefund", true, createdAt, false },
                    { 11L, 2, "participants", "export", true, createdAt, false },
                    { 12L, 1, "participants", "list", true, createdAt, false },
                    { 13L, 1, "participants", "viewDetails", true, createdAt, false },
                    { 14L, 1, "participants", "viewContactInfo", true, createdAt, false },
                    { 15L, 1, "participants", "editProfile", false, createdAt, false },
                    { 16L, 1, "participants", "viewFinance", false, createdAt, false },
                    { 17L, 1, "participants", "viewOrder", true, createdAt, false },
                    { 18L, 1, "participants", "resendProfileLink", true, createdAt, false },
                    { 19L, 1, "participants", "changeStatus", false, createdAt, false },
                    { 20L, 1, "participants", "replaceParticipant", false, createdAt, false },
                    { 21L, 1, "participants", "emergencyRefund", false, createdAt, false },
                    { 22L, 1, "participants", "export", false, createdAt, false },
                    { 23L, 3, "participants", "list", true, createdAt, false },
                    { 24L, 3, "participants", "viewDetails", true, createdAt, false },
                    { 25L, 3, "participants", "viewContactInfo", true, createdAt, false },
                    { 26L, 3, "participants", "editProfile", false, createdAt, false },
                    { 27L, 3, "participants", "viewFinance", false, createdAt, false },
                    { 28L, 3, "participants", "viewOrder", true, createdAt, false },
                    { 29L, 3, "participants", "resendProfileLink", true, createdAt, false },
                    { 30L, 3, "participants", "changeStatus", true, createdAt, false },
                    { 31L, 3, "participants", "replaceParticipant", false, createdAt, false },
                    { 32L, 3, "participants", "emergencyRefund", false, createdAt, false },
                    { 33L, 3, "participants", "export", false, createdAt, false },
                    { 34L, 2, "events", "viewParticipants", true, createdAt, false },
                    { 35L, 1, "events", "viewParticipants", true, createdAt, false },
                    { 36L, 3, "events", "viewParticipants", true, createdAt, false },
                    { 37L, 2, "orders", "view", true, createdAt, false },
                    { 38L, 1, "orders", "view", true, createdAt, false },
                    { 39L, 3, "orders", "view", true, createdAt, false },
                    { 40L, 2, "users", "manageOperationPermissions", true, createdAt, false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleOperationPermissions");

            migrationBuilder.DropTable(
                name: "UserOperationPermissionOverrides");

            migrationBuilder.DropTable(
                name: "PermissionActions");
        }
    }
}
