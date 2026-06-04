using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEventSmsSchedulingAndAdminEdits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PlannedSendAtUtc",
                table: "SmsQueueItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedMessage",
                table: "EventParticipantSmsRequests",
                type: "nvarchar(480)",
                maxLength: 480,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlannedSendAtUtc",
                table: "EventParticipantSmsRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SmsQueueItems_PlannedSendAtUtc",
                table: "SmsQueueItems",
                column: "PlannedSendAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipantSmsRequests_PlannedSendAtUtc",
                table: "EventParticipantSmsRequests",
                column: "PlannedSendAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SmsQueueItems_PlannedSendAtUtc",
                table: "SmsQueueItems");

            migrationBuilder.DropIndex(
                name: "IX_EventParticipantSmsRequests_PlannedSendAtUtc",
                table: "EventParticipantSmsRequests");

            migrationBuilder.DropColumn(
                name: "PlannedSendAtUtc",
                table: "SmsQueueItems");

            migrationBuilder.DropColumn(
                name: "ApprovedMessage",
                table: "EventParticipantSmsRequests");

            migrationBuilder.DropColumn(
                name: "PlannedSendAtUtc",
                table: "EventParticipantSmsRequests");
        }
    }
}
