using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionActionCatalogMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EntityLabel",
                table: "PermissionActions",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GroupKey",
                table: "PermissionActions",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GroupLabel",
                table: "PermissionActions",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HandlerName",
                table: "PermissionActions",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeprecated",
                table: "PermissionActions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystemAction",
                table: "PermissionActions",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "PagePath",
                table: "PermissionActions",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RiskLevel",
                table: "PermissionActions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Low");

            migrationBuilder.AddColumn<string>(
                name: "UiSurface",
                table: "PermissionActions",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "Manual");

            migrationBuilder.Sql("""
                UPDATE PermissionActions
                SET
                    EntityLabel = CASE Entity
                        WHEN N'participants' THEN N'شرکت‌کنندگان'
                        WHEN N'events' THEN N'رویدادها'
                        WHEN N'orders' THEN N'سفارش‌ها و خریداران'
                        WHEN N'users' THEN N'کاربران پنل'
                        ELSE Entity
                    END,
                    GroupKey = CASE
                        WHEN Entity IN (N'participants', N'users') THEN N'users'
                        WHEN Entity IN (N'orders') THEN N'orders'
                        WHEN Entity IN (N'events') THEN N'events'
                        ELSE Entity
                    END,
                    GroupLabel = CASE
                        WHEN Entity IN (N'participants', N'users') THEN N'کاربران و پروفایل‌ها'
                        WHEN Entity IN (N'orders') THEN N'سفارش‌ها و خرید'
                        WHEN Entity IN (N'events') THEN N'رویدادها'
                        ELSE Entity
                    END,
                    UiSurface = N'Manual',
                    RiskLevel = N'Medium',
                    IsSystemAction = CAST(1 AS bit),
                    IsDeprecated = CAST(0 AS bit)
                WHERE EntityLabel = N'';
                """);

            migrationBuilder.CreateIndex(
                name: "IX_PermissionActions_GroupKey_DisplayOrder",
                table: "PermissionActions",
                columns: new[] { "GroupKey", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_PermissionActions_RiskLevel",
                table: "PermissionActions",
                column: "RiskLevel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PermissionActions_GroupKey_DisplayOrder",
                table: "PermissionActions");

            migrationBuilder.DropIndex(
                name: "IX_PermissionActions_RiskLevel",
                table: "PermissionActions");

            migrationBuilder.DropColumn(
                name: "EntityLabel",
                table: "PermissionActions");

            migrationBuilder.DropColumn(
                name: "GroupKey",
                table: "PermissionActions");

            migrationBuilder.DropColumn(
                name: "GroupLabel",
                table: "PermissionActions");

            migrationBuilder.DropColumn(
                name: "HandlerName",
                table: "PermissionActions");

            migrationBuilder.DropColumn(
                name: "IsDeprecated",
                table: "PermissionActions");

            migrationBuilder.DropColumn(
                name: "IsSystemAction",
                table: "PermissionActions");

            migrationBuilder.DropColumn(
                name: "PagePath",
                table: "PermissionActions");

            migrationBuilder.DropColumn(
                name: "RiskLevel",
                table: "PermissionActions");

            migrationBuilder.DropColumn(
                name: "UiSurface",
                table: "PermissionActions");
        }
    }
}
