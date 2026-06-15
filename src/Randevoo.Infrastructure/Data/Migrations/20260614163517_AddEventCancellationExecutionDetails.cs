using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEventCancellationExecutionDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExecutedAtUtc",
                table: "EventCancellationRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviewJson",
                table: "EventCancellationRequests",
                type: "nvarchar(max)",
                maxLength: 8000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicMessage",
                table: "EventCancellationRequests",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExecutedAtUtc",
                table: "EventCancellationRequests");

            migrationBuilder.DropColumn(
                name: "PreviewJson",
                table: "EventCancellationRequests");

            migrationBuilder.DropColumn(
                name: "PublicMessage",
                table: "EventCancellationRequests");
        }
    }
}
