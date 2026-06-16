using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserReportModerationRestrictions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRestrictions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RestrictionType = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RemovedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    RemovedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RemovalReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRestrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRestrictions_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRestrictions_Users_RemovedByUserId",
                        column: x => x.RemovedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRestrictions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModerationReports_ReportedUserId_Status_CreatedAt",
                table: "ModerationReports",
                columns: new[] { "ReportedUserId", "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ModerationReports_ReporterUserId_ReportedUserId_DatingEventId_Status",
                table: "ModerationReports",
                columns: new[] { "ReporterUserId", "ReportedUserId", "DatingEventId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_UserRestrictions_CreatedByUserId",
                table: "UserRestrictions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRestrictions_ExpiresAtUtc",
                table: "UserRestrictions",
                column: "ExpiresAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_UserRestrictions_RemovedByUserId",
                table: "UserRestrictions",
                column: "RemovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRestrictions_UserId_RestrictionType_IsActive",
                table: "UserRestrictions",
                columns: new[] { "UserId", "RestrictionType", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRestrictions");

            migrationBuilder.DropIndex(
                name: "IX_ModerationReports_ReportedUserId_Status_CreatedAt",
                table: "ModerationReports");

            migrationBuilder.DropIndex(
                name: "IX_ModerationReports_ReporterUserId_ReportedUserId_DatingEventId_Status",
                table: "ModerationReports");
        }
    }
}
