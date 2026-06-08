using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSupportTicketLookupTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupportTicketCategories",
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
                    table.PrimaryKey("PK_SupportTicketCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupportTicketStatuses",
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
                    table.PrimaryKey("PK_SupportTicketStatuses", x => x.Id);
                });

            var seededAt = new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc);
            migrationBuilder.InsertData(
                table: "SupportTicketCategories",
                columns: new[] { "Id", "Name", "DisplayNameFa", "IsActive", "DisplayOrder", "CreatedAt", "UpdatedAt", "IsDeleted", "DeletedAt" },
                values: new object[,]
                {
                    { 1L, "financial-problem", "مشکل مالی", true, 1, seededAt, null, false, null },
                    { 2L, "event-problem", "مشکل رویداد", true, 2, seededAt, null, false, null },
                    { 3L, "general-question", "سوال عمومی", true, 3, seededAt, null, false, null }
                });

            migrationBuilder.InsertData(
                table: "SupportTicketStatuses",
                columns: new[] { "Id", "Name", "DisplayNameFa", "IsActive", "DisplayOrder", "CreatedAt", "UpdatedAt", "IsDeleted", "DeletedAt" },
                values: new object[,]
                {
                    { 1L, "open", "باز", true, 1, seededAt, null, false, null },
                    { 2L, "in-progress", "در حال رسیدگی", true, 2, seededAt, null, false, null },
                    { 3L, "waiting-for-user", "منتظر ثبت‌کننده", true, 3, seededAt, null, false, null },
                    { 4L, "closed", "بسته", true, 4, seededAt, null, false, null },
                    { 5L, "reopened", "بازگشایی شده", true, 5, seededAt, null, false, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupportTicketCategories_Name",
                table: "SupportTicketCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupportTicketStatuses_Name",
                table: "SupportTicketStatuses",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupportTicketCategories");

            migrationBuilder.DropTable(
                name: "SupportTicketStatuses");
        }
    }
}
