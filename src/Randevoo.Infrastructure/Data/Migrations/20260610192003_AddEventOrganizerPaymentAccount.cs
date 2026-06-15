using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEventOrganizerPaymentAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OrganizerPaymentAccountId",
                table: "DatingEvents",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DatingEvents_OrganizerPaymentAccountId",
                table: "DatingEvents",
                column: "OrganizerPaymentAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_DatingEvents_PlannerBankAccounts_OrganizerPaymentAccountId",
                table: "DatingEvents",
                column: "OrganizerPaymentAccountId",
                principalTable: "PlannerBankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatingEvents_PlannerBankAccounts_OrganizerPaymentAccountId",
                table: "DatingEvents");

            migrationBuilder.DropIndex(
                name: "IX_DatingEvents_OrganizerPaymentAccountId",
                table: "DatingEvents");

            migrationBuilder.DropColumn(
                name: "OrganizerPaymentAccountId",
                table: "DatingEvents");
        }
    }
}
