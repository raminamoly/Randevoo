using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEventDeliveryModesAndFaqs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EventModeId",
                table: "DatingEvents",
                type: "bigint",
                nullable: false,
                defaultValue: 2L);

            migrationBuilder.AddColumn<string>(
                name: "OnlineAccessInstructions",
                table: "DatingEvents",
                type: "nvarchar(1200)",
                maxLength: 1200,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OnlineEventPlatformId",
                table: "DatingEvents",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OnlineJoinUrl",
                table: "DatingEvents",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EventFaqs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatingEventId = table.Column<long>(type: "bigint", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(1200)", maxLength: 1200, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventFaqs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventFaqs_DatingEvents_DatingEventId",
                        column: x => x.DatingEventId,
                        principalTable: "DatingEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventModes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    IsOnline = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventModes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OnlineEventPlatforms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnlineEventPlatforms", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "EventModes",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "DisplayOrder", "IsActive", "IsDeleted", "IsOnline", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, false, true, "آنلاین", null },
                    { 2L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, false, false, "حضوری", null }
                });

            migrationBuilder.InsertData(
                table: "OnlineEventPlatforms",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "DisplayOrder", "IsActive", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, false, "Google Meet", null },
                    { 2L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, false, "Zoom", null },
                    { 3L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, false, "اسکای روم", null },
                    { 4L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 4, true, false, "ادوبی کانکت", null },
                    { 5L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 5, true, false, "سایر", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatingEvents_EventModeId",
                table: "DatingEvents",
                column: "EventModeId");

            migrationBuilder.CreateIndex(
                name: "IX_DatingEvents_OnlineEventPlatformId",
                table: "DatingEvents",
                column: "OnlineEventPlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_EventFaqs_DatingEventId_DisplayOrder",
                table: "EventFaqs",
                columns: new[] { "DatingEventId", "DisplayOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventModes_Name",
                table: "EventModes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OnlineEventPlatforms_Name",
                table: "OnlineEventPlatforms",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DatingEvents_EventModes_EventModeId",
                table: "DatingEvents",
                column: "EventModeId",
                principalTable: "EventModes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DatingEvents_OnlineEventPlatforms_OnlineEventPlatformId",
                table: "DatingEvents",
                column: "OnlineEventPlatformId",
                principalTable: "OnlineEventPlatforms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatingEvents_EventModes_EventModeId",
                table: "DatingEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_DatingEvents_OnlineEventPlatforms_OnlineEventPlatformId",
                table: "DatingEvents");

            migrationBuilder.DropTable(
                name: "EventFaqs");

            migrationBuilder.DropTable(
                name: "EventModes");

            migrationBuilder.DropTable(
                name: "OnlineEventPlatforms");

            migrationBuilder.DropIndex(
                name: "IX_DatingEvents_EventModeId",
                table: "DatingEvents");

            migrationBuilder.DropIndex(
                name: "IX_DatingEvents_OnlineEventPlatformId",
                table: "DatingEvents");

            migrationBuilder.DropColumn(
                name: "EventModeId",
                table: "DatingEvents");

            migrationBuilder.DropColumn(
                name: "OnlineAccessInstructions",
                table: "DatingEvents");

            migrationBuilder.DropColumn(
                name: "OnlineEventPlatformId",
                table: "DatingEvents");

            migrationBuilder.DropColumn(
                name: "OnlineJoinUrl",
                table: "DatingEvents");
        }
    }
}
