using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class EventParticipantsChatsAndSurveys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRemoved",
                table: "EventTickets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RemovalReason",
                table: "EventTickets",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RemovedAt",
                table: "EventTickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RemovedByUserId",
                table: "EventTickets",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EventConversations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatingEventId = table.Column<long>(type: "bigint", nullable: false),
                    StarterUserId = table.Column<long>(type: "bigint", nullable: false),
                    ParticipantUserId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventConversations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventConversations_DatingEvents_DatingEventId",
                        column: x => x.DatingEventId,
                        principalTable: "DatingEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventConversations_Users_ParticipantUserId",
                        column: x => x.ParticipantUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventConversations_Users_StarterUserId",
                        column: x => x.StarterUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventSurveyResponses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatingEventId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSurveyResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventSurveyResponses_DatingEvents_DatingEventId",
                        column: x => x.DatingEventId,
                        principalTable: "DatingEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventSurveyResponses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventChatBlocks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventConversationId = table.Column<long>(type: "bigint", nullable: false),
                    BlockerUserId = table.Column<long>(type: "bigint", nullable: false),
                    BlockedUserId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventChatBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventChatBlocks_EventConversations_EventConversationId",
                        column: x => x.EventConversationId,
                        principalTable: "EventConversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventChatBlocks_Users_BlockedUserId",
                        column: x => x.BlockedUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventChatBlocks_Users_BlockerUserId",
                        column: x => x.BlockerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventChatMessages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventConversationId = table.Column<long>(type: "bigint", nullable: false),
                    SenderUserId = table.Column<long>(type: "bigint", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventChatMessages_EventConversations_EventConversationId",
                        column: x => x.EventConversationId,
                        principalTable: "EventConversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventChatMessages_Users_SenderUserId",
                        column: x => x.SenderUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventSurveyRatings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventSurveyResponseId = table.Column<long>(type: "bigint", nullable: false),
                    Factor = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSurveyRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventSurveyRatings_EventSurveyResponses_EventSurveyResponseId",
                        column: x => x.EventSurveyResponseId,
                        principalTable: "EventSurveyResponses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventChatBlocks_BlockedUserId",
                table: "EventChatBlocks",
                column: "BlockedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventChatBlocks_BlockerUserId",
                table: "EventChatBlocks",
                column: "BlockerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventChatBlocks_EventConversationId_BlockerUserId_BlockedUserId",
                table: "EventChatBlocks",
                columns: new[] { "EventConversationId", "BlockerUserId", "BlockedUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventChatMessages_EventConversationId",
                table: "EventChatMessages",
                column: "EventConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_EventChatMessages_SenderUserId",
                table: "EventChatMessages",
                column: "SenderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventConversations_DatingEventId_StarterUserId_ParticipantUserId",
                table: "EventConversations",
                columns: new[] { "DatingEventId", "StarterUserId", "ParticipantUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventConversations_ParticipantUserId",
                table: "EventConversations",
                column: "ParticipantUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventConversations_StarterUserId",
                table: "EventConversations",
                column: "StarterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSurveyRatings_EventSurveyResponseId_Factor",
                table: "EventSurveyRatings",
                columns: new[] { "EventSurveyResponseId", "Factor" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventSurveyResponses_DatingEventId_UserId",
                table: "EventSurveyResponses",
                columns: new[] { "DatingEventId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventSurveyResponses_UserId",
                table: "EventSurveyResponses",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventChatBlocks");

            migrationBuilder.DropTable(
                name: "EventChatMessages");

            migrationBuilder.DropTable(
                name: "EventSurveyRatings");

            migrationBuilder.DropTable(
                name: "EventConversations");

            migrationBuilder.DropTable(
                name: "EventSurveyResponses");

            migrationBuilder.DropColumn(
                name: "IsRemoved",
                table: "EventTickets");

            migrationBuilder.DropColumn(
                name: "RemovalReason",
                table: "EventTickets");

            migrationBuilder.DropColumn(
                name: "RemovedAt",
                table: "EventTickets");

            migrationBuilder.DropColumn(
                name: "RemovedByUserId",
                table: "EventTickets");
        }
    }
}
