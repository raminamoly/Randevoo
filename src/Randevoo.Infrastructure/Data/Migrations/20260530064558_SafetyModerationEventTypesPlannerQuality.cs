using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SafetyModerationEventTypesPlannerQuality : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AverageRating",
                table: "EventPlannerProfiles",
                type: "decimal(3,2)",
                precision: 3,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "CancelledEventCount",
                table: "EventPlannerProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompletedEventCount",
                table: "EventPlannerProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HostedEventCount",
                table: "EventPlannerProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalSurveyCount",
                table: "EventPlannerProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DisabledAt",
                table: "EventConversations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DisabledByUserId",
                table: "EventConversations",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisabledReason",
                table: "EventConversations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDisabled",
                table: "EventConversations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "BalanceTransactions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ReferenceId",
                table: "BalanceTransactions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceType",
                table: "BalanceTransactions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EventTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModerationReports",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReporterUserId = table.Column<long>(type: "bigint", nullable: false),
                    ReportedUserId = table.Column<long>(type: "bigint", nullable: false),
                    DatingEventId = table.Column<long>(type: "bigint", nullable: true),
                    EventConversationId = table.Column<long>(type: "bigint", nullable: true),
                    Reason = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AdminReviewNote = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ReviewedByAdminUserId = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModerationReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModerationReports_DatingEvents_DatingEventId",
                        column: x => x.DatingEventId,
                        principalTable: "DatingEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ModerationReports_EventConversations_EventConversationId",
                        column: x => x.EventConversationId,
                        principalTable: "EventConversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ModerationReports_Users_ReportedUserId",
                        column: x => x.ReportedUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ModerationReports_Users_ReporterUserId",
                        column: x => x.ReporterUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ModerationReports_Users_ReviewedByAdminUserId",
                        column: x => x.ReviewedByAdminUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "EventTypes",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Description", "IsActive", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 5, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Social deduction event", true, false, "Mafia", null },
                    { 2L, new DateTime(2026, 5, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Board game social event", true, false, "Board Game", null },
                    { 3L, new DateTime(2026, 5, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Poetry and conversation event", true, false, "Poem Reading", null },
                    { 4L, new DateTime(2026, 5, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Casual cafe meetup", true, false, "Cafe Meetup", null },
                    { 5L, new DateTime(2026, 5, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Outdoor hiking event", true, false, "Hiking", null },
                    { 6L, new DateTime(2026, 5, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Structured short introductions", true, false, "Speed Dating", null },
                    { 7L, new DateTime(2026, 5, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Competitive game tournament", true, false, "Game Tournament", null },
                    { 8L, new DateTime(2026, 5, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Learning-focused social workshop", true, false, "Workshop", null },
                    { 9L, new DateTime(2026, 5, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Art and creativity event", true, false, "Art Night", null },
                    { 10L, new DateTime(2026, 5, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Music-focused social event", true, false, "Music Night", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventTypes_Name",
                table: "EventTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModerationReports_DatingEventId",
                table: "ModerationReports",
                column: "DatingEventId");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationReports_EventConversationId",
                table: "ModerationReports",
                column: "EventConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationReports_ReportedUserId",
                table: "ModerationReports",
                column: "ReportedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationReports_ReporterUserId",
                table: "ModerationReports",
                column: "ReporterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationReports_ReviewedByAdminUserId",
                table: "ModerationReports",
                column: "ReviewedByAdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationReports_Status",
                table: "ModerationReports",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventTypes");

            migrationBuilder.DropTable(
                name: "ModerationReports");

            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "EventPlannerProfiles");

            migrationBuilder.DropColumn(
                name: "CancelledEventCount",
                table: "EventPlannerProfiles");

            migrationBuilder.DropColumn(
                name: "CompletedEventCount",
                table: "EventPlannerProfiles");

            migrationBuilder.DropColumn(
                name: "HostedEventCount",
                table: "EventPlannerProfiles");

            migrationBuilder.DropColumn(
                name: "TotalSurveyCount",
                table: "EventPlannerProfiles");

            migrationBuilder.DropColumn(
                name: "DisabledAt",
                table: "EventConversations");

            migrationBuilder.DropColumn(
                name: "DisabledByUserId",
                table: "EventConversations");

            migrationBuilder.DropColumn(
                name: "DisabledReason",
                table: "EventConversations");

            migrationBuilder.DropColumn(
                name: "IsDisabled",
                table: "EventConversations");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "BalanceTransactions");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "BalanceTransactions");

            migrationBuilder.DropColumn(
                name: "ReferenceType",
                table: "BalanceTransactions");
        }
    }
}
