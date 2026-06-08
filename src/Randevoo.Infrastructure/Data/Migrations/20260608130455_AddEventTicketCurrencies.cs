using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEventTicketCurrencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "EventTickets",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "IRR");

            migrationBuilder.AddColumn<string>(
                name: "FemaleTicketCurrencyCode",
                table: "DatingEvents",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "IRR");

            migrationBuilder.AddColumn<string>(
                name: "MaleTicketCurrencyCode",
                table: "DatingEvents",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "IRR");

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    DisplayNameFa = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "Code", "CreatedAt", "DeletedAt", "DisplayNameFa", "DisplayOrder", "IsActive", "IsDeleted", "Symbol", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, "IRR", new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, "ریال ایران", 1, true, false, "ریال", null },
                    { 2L, "EUR", new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, "یورو", 2, true, false, "€", null },
                    { 3L, "USD", new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, "دلار آمریکا", 3, true, false, "$", null },
                    { 4L, "CAD", new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, "دلار کانادا", 4, true, false, "C$", null },
                    { 5L, "GBP", new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, "پوند انگلیس", 5, true, false, "£", null },
                    { 6L, "AED", new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, "درهم امارات", 6, true, false, "AED", null },
                    { 7L, "TRY", new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, "لیر ترکیه", 7, true, false, "₺", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventTickets_CurrencyCode",
                table: "EventTickets",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_DatingEvents_FemaleTicketCurrencyCode",
                table: "DatingEvents",
                column: "FemaleTicketCurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_DatingEvents_MaleTicketCurrencyCode",
                table: "DatingEvents",
                column: "MaleTicketCurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Code",
                table: "Currencies",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropIndex(
                name: "IX_EventTickets_CurrencyCode",
                table: "EventTickets");

            migrationBuilder.DropIndex(
                name: "IX_DatingEvents_FemaleTicketCurrencyCode",
                table: "DatingEvents");

            migrationBuilder.DropIndex(
                name: "IX_DatingEvents_MaleTicketCurrencyCode",
                table: "DatingEvents");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "EventTickets");

            migrationBuilder.DropColumn(
                name: "FemaleTicketCurrencyCode",
                table: "DatingEvents");

            migrationBuilder.DropColumn(
                name: "MaleTicketCurrencyCode",
                table: "DatingEvents");
        }
    }
}
