using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPlannerProfileApprovalWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasPendingChanges",
                table: "EventPlannerProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PendingCity",
                table: "EventPlannerProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PendingFullName",
                table: "EventPlannerProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PendingPictureUrl",
                table: "EventPlannerProfiles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PendingResume",
                table: "EventPlannerProfiles",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PendingReviewNote",
                table: "EventPlannerProfiles",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PendingReviewedAt",
                table: "EventPlannerProfiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PendingReviewedByAdminUserId",
                table: "EventPlannerProfiles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PendingSubmittedAt",
                table: "EventPlannerProfiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PendingTitle",
                table: "EventPlannerProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasPendingChanges",
                table: "EventPlannerProfiles");

            migrationBuilder.DropColumn(
                name: "PendingCity",
                table: "EventPlannerProfiles");

            migrationBuilder.DropColumn(
                name: "PendingFullName",
                table: "EventPlannerProfiles");

            migrationBuilder.DropColumn(
                name: "PendingPictureUrl",
                table: "EventPlannerProfiles");

            migrationBuilder.DropColumn(
                name: "PendingResume",
                table: "EventPlannerProfiles");

            migrationBuilder.DropColumn(
                name: "PendingReviewNote",
                table: "EventPlannerProfiles");

            migrationBuilder.DropColumn(
                name: "PendingReviewedAt",
                table: "EventPlannerProfiles");

            migrationBuilder.DropColumn(
                name: "PendingReviewedByAdminUserId",
                table: "EventPlannerProfiles");

            migrationBuilder.DropColumn(
                name: "PendingSubmittedAt",
                table: "EventPlannerProfiles");

            migrationBuilder.DropColumn(
                name: "PendingTitle",
                table: "EventPlannerProfiles");
        }
    }
}
