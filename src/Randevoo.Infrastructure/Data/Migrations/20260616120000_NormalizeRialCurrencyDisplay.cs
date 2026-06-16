using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Randevoo.Infrastructure.Data.Migrations
{
    public partial class NormalizeRialCurrencyDisplay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CurrencyLookups",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "DisplayNameFa", "Symbol" },
                values: new object[] { "ریال", "ریال" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CurrencyLookups",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "DisplayNameFa", "Symbol" },
                values: new object[] { "ریال ایران", "ریال" });
        }
    }
}
