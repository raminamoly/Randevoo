using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSupportTicketRecipientsAndLookupIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DatingEventId",
                table: "SupportTickets",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RecipientPlannerUserId",
                table: "SupportTickets",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TicketRecipientTypeId",
                table: "SupportTickets",
                type: "bigint",
                nullable: false,
                defaultValue: 1L);

            migrationBuilder.AddColumn<long>(
                name: "TicketStatusId",
                table: "SupportTickets",
                type: "bigint",
                nullable: false,
                defaultValue: 1L);

            migrationBuilder.AddColumn<long>(
                name: "TicketTypeId",
                table: "SupportTickets",
                type: "bigint",
                nullable: false,
                defaultValue: 3L);

            migrationBuilder.CreateTable(
                name: "SupportTicketRecipientTypes",
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
                    table.PrimaryKey("PK_SupportTicketRecipientTypes", x => x.Id);
                });

            var seededAt = new DateTime(2026, 6, 9, 0, 0, 0, 0, DateTimeKind.Utc);
            migrationBuilder.InsertData(
                table: "SupportTicketCategories",
                columns: new[] { "Id", "Name", "DisplayNameFa", "IsActive", "DisplayOrder", "CreatedAt", "UpdatedAt", "IsDeleted", "DeletedAt" },
                values: new object[,]
                {
                    { 4L, "ticket-problem", "مشکل تیکت", true, 4, seededAt, null, false, null },
                    { 5L, "pre-purchase-question", "سوال پیش از خرید", true, 5, seededAt, null, false, null }
                });

            migrationBuilder.InsertData(
                table: "SupportTicketRecipientTypes",
                columns: new[] { "Id", "Name", "DisplayNameFa", "IsActive", "DisplayOrder", "CreatedAt", "UpdatedAt", "IsDeleted", "DeletedAt" },
                values: new object[,]
                {
                    { 1L, "platform-support", "پشتیبانی سایت", true, 1, seededAt, null, false, null },
                    { 2L, "event-planner", "برگزارکننده رویداد", true, 2, seededAt, null, false, null }
                });

            migrationBuilder.Sql(
                """
                UPDATE SupportTickets
                SET TicketTypeId = CASE Category
                    WHEN 0 THEN 1
                    WHEN 1 THEN 2
                    WHEN 2 THEN 3
                    ELSE 3
                END,
                TicketStatusId = CASE Status
                    WHEN 0 THEN 1
                    WHEN 1 THEN 2
                    WHEN 2 THEN 3
                    WHEN 3 THEN 4
                    WHEN 4 THEN 5
                    ELSE 1
                END,
                TicketRecipientTypeId = 1
                """);

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_DatingEventId",
                table: "SupportTickets",
                column: "DatingEventId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_RecipientPlannerUserId",
                table: "SupportTickets",
                column: "RecipientPlannerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_TicketRecipientTypeId",
                table: "SupportTickets",
                column: "TicketRecipientTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_TicketStatusId_TicketTypeId_TicketRecipientTypeId_CreatedAt",
                table: "SupportTickets",
                columns: new[] { "TicketStatusId", "TicketTypeId", "TicketRecipientTypeId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_TicketTypeId",
                table: "SupportTickets",
                column: "TicketTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTicketRecipientTypes_Name",
                table: "SupportTicketRecipientTypes",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SupportTickets_DatingEvents_DatingEventId",
                table: "SupportTickets",
                column: "DatingEventId",
                principalTable: "DatingEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupportTickets_SupportTicketCategories_TicketTypeId",
                table: "SupportTickets",
                column: "TicketTypeId",
                principalTable: "SupportTicketCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupportTickets_SupportTicketRecipientTypes_TicketRecipientTypeId",
                table: "SupportTickets",
                column: "TicketRecipientTypeId",
                principalTable: "SupportTicketRecipientTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupportTickets_SupportTicketStatuses_TicketStatusId",
                table: "SupportTickets",
                column: "TicketStatusId",
                principalTable: "SupportTicketStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupportTickets_Users_RecipientPlannerUserId",
                table: "SupportTickets",
                column: "RecipientPlannerUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportTickets_DatingEvents_DatingEventId",
                table: "SupportTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_SupportTickets_SupportTicketCategories_TicketTypeId",
                table: "SupportTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_SupportTickets_SupportTicketRecipientTypes_TicketRecipientTypeId",
                table: "SupportTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_SupportTickets_SupportTicketStatuses_TicketStatusId",
                table: "SupportTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_SupportTickets_Users_RecipientPlannerUserId",
                table: "SupportTickets");

            migrationBuilder.DropTable(
                name: "SupportTicketRecipientTypes");

            migrationBuilder.DeleteData(
                table: "SupportTicketCategories",
                keyColumn: "Id",
                keyValues: new object[] { 4L, 5L });

            migrationBuilder.DropIndex(
                name: "IX_SupportTickets_DatingEventId",
                table: "SupportTickets");

            migrationBuilder.DropIndex(
                name: "IX_SupportTickets_RecipientPlannerUserId",
                table: "SupportTickets");

            migrationBuilder.DropIndex(
                name: "IX_SupportTickets_TicketRecipientTypeId",
                table: "SupportTickets");

            migrationBuilder.DropIndex(
                name: "IX_SupportTickets_TicketStatusId_TicketTypeId_TicketRecipientTypeId_CreatedAt",
                table: "SupportTickets");

            migrationBuilder.DropIndex(
                name: "IX_SupportTickets_TicketTypeId",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "DatingEventId",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "RecipientPlannerUserId",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "TicketRecipientTypeId",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "TicketStatusId",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "TicketTypeId",
                table: "SupportTickets");
        }
    }
}
