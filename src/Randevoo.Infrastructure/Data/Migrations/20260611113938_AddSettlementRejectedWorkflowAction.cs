using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSettlementRejectedWorkflowAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "EventWorkflowActionTypes",
                keyColumn: "Id",
                keyValue: 15L,
                columns: new[] { "DisplayNameFa", "Name" },
                values: new object[] { "رد تسویه رویداد", "SettlementRejected" });

            migrationBuilder.UpdateData(
                table: "EventWorkflowActionTypes",
                keyColumn: "Id",
                keyValue: 16L,
                columns: new[] { "DisplayNameFa", "Name" },
                values: new object[] { "بستانکاری برگزارکننده", "OrganizerCredited" });

            migrationBuilder.InsertData(
                table: "EventWorkflowActionTypes",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "DisplayNameFa", "DisplayOrder", "IsActive", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[] { 17L, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "درخواست برداشت", 17, true, false, "WithdrawalRequested", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EventWorkflowActionTypes",
                keyColumn: "Id",
                keyValue: 17L);

            migrationBuilder.UpdateData(
                table: "EventWorkflowActionTypes",
                keyColumn: "Id",
                keyValue: 15L,
                columns: new[] { "DisplayNameFa", "Name" },
                values: new object[] { "بستانکاری برگزارکننده", "OrganizerCredited" });

            migrationBuilder.UpdateData(
                table: "EventWorkflowActionTypes",
                keyColumn: "Id",
                keyValue: 16L,
                columns: new[] { "DisplayNameFa", "Name" },
                values: new object[] { "درخواست برداشت", "WithdrawalRequested" });
        }
    }
}
