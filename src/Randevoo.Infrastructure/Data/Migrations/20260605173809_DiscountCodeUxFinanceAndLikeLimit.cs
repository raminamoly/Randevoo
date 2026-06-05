using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class DiscountCodeUxFinanceAndLikeLimit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventDiscountCodes_DatingEvents_DatingEventId",
                table: "EventDiscountCodes");

            migrationBuilder.DropIndex(
                name: "IX_EventDiscountCodes_DatingEventId_Code",
                table: "EventDiscountCodes");

            migrationBuilder.RenameColumn(
                name: "NumberOfChatAllowed",
                table: "DatingEvents",
                newName: "NumberOfLikesAllowed");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "EventTickets",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DiscountCode",
                table: "EventTickets",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EventDiscountCodeId",
                table: "EventTickets",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalPrice",
                table: "EventTickets",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql("UPDATE EventTickets SET OriginalPrice = Price WHERE OriginalPrice = 0");

            migrationBuilder.AlterColumn<long>(
                name: "DatingEventId",
                table: "EventDiscountCodes",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_EventTickets_EventDiscountCodeId",
                table: "EventTickets",
                column: "EventDiscountCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_EventDiscountCodes_DatingEventId_Code",
                table: "EventDiscountCodes",
                columns: new[] { "DatingEventId", "Code" },
                unique: true,
                filter: "[DatingEventId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EventDiscountCodes_Global_Code",
                table: "EventDiscountCodes",
                column: "Code",
                unique: true,
                filter: "[DatingEventId] IS NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_EventDiscountCodes_DatingEvents_DatingEventId",
                table: "EventDiscountCodes",
                column: "DatingEventId",
                principalTable: "DatingEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventTickets_EventDiscountCodes_EventDiscountCodeId",
                table: "EventTickets",
                column: "EventDiscountCodeId",
                principalTable: "EventDiscountCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventDiscountCodes_DatingEvents_DatingEventId",
                table: "EventDiscountCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_EventTickets_EventDiscountCodes_EventDiscountCodeId",
                table: "EventTickets");

            migrationBuilder.DropIndex(
                name: "IX_EventTickets_EventDiscountCodeId",
                table: "EventTickets");

            migrationBuilder.DropIndex(
                name: "IX_EventDiscountCodes_DatingEventId_Code",
                table: "EventDiscountCodes");

            migrationBuilder.DropIndex(
                name: "IX_EventDiscountCodes_Global_Code",
                table: "EventDiscountCodes");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "EventTickets");

            migrationBuilder.DropColumn(
                name: "DiscountCode",
                table: "EventTickets");

            migrationBuilder.DropColumn(
                name: "EventDiscountCodeId",
                table: "EventTickets");

            migrationBuilder.DropColumn(
                name: "OriginalPrice",
                table: "EventTickets");

            migrationBuilder.RenameColumn(
                name: "NumberOfLikesAllowed",
                table: "DatingEvents",
                newName: "NumberOfChatAllowed");

            migrationBuilder.AlterColumn<long>(
                name: "DatingEventId",
                table: "EventDiscountCodes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventDiscountCodes_DatingEventId_Code",
                table: "EventDiscountCodes",
                columns: new[] { "DatingEventId", "Code" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EventDiscountCodes_DatingEvents_DatingEventId",
                table: "EventDiscountCodes",
                column: "DatingEventId",
                principalTable: "DatingEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
