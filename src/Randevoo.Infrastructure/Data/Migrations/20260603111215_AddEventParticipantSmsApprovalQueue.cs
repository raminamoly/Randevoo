using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEventParticipantSmsApprovalQueue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventParticipantSmsRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatingEventId = table.Column<long>(type: "bigint", nullable: false),
                    RequestedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(480)", maxLength: 480, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReviewNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewedByAdminUserId = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QueuedRecipientsCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventParticipantSmsRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventParticipantSmsRequests_DatingEvents_DatingEventId",
                        column: x => x.DatingEventId,
                        principalTable: "DatingEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventParticipantSmsRequests_Users_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventParticipantSmsRequests_Users_ReviewedByAdminUserId",
                        column: x => x.ReviewedByAdminUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SmsQueueItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventParticipantSmsRequestId = table.Column<long>(type: "bigint", nullable: true),
                    DatingEventId = table.Column<long>(type: "bigint", nullable: false),
                    RecipientUserId = table.Column<long>(type: "bigint", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(480)", maxLength: 480, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsQueueItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmsQueueItems_DatingEvents_DatingEventId",
                        column: x => x.DatingEventId,
                        principalTable: "DatingEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SmsQueueItems_EventParticipantSmsRequests_EventParticipantSmsRequestId",
                        column: x => x.EventParticipantSmsRequestId,
                        principalTable: "EventParticipantSmsRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_SmsQueueItems_Users_RecipientUserId",
                        column: x => x.RecipientUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipantSmsRequests_DatingEventId_Status_CreatedAt",
                table: "EventParticipantSmsRequests",
                columns: new[] { "DatingEventId", "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipantSmsRequests_RequestedByUserId",
                table: "EventParticipantSmsRequests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipantSmsRequests_ReviewedByAdminUserId",
                table: "EventParticipantSmsRequests",
                column: "ReviewedByAdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsQueueItems_DatingEventId",
                table: "SmsQueueItems",
                column: "DatingEventId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsQueueItems_EventParticipantSmsRequestId",
                table: "SmsQueueItems",
                column: "EventParticipantSmsRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsQueueItems_RecipientUserId",
                table: "SmsQueueItems",
                column: "RecipientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsQueueItems_Status_CreatedAt",
                table: "SmsQueueItems",
                columns: new[] { "Status", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SmsQueueItems");

            migrationBuilder.DropTable(
                name: "EventParticipantSmsRequests");
        }
    }
}
