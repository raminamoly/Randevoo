using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEventDiscountCodesAndGenderTicketPricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TicketPrice",
                table: "DatingEvents",
                newName: "MaleTicketPrice");

            migrationBuilder.AddColumn<decimal>(
                name: "FemaleTicketPrice",
                table: "DatingEvents",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql(
                """
                UPDATE DatingEvents
                SET FemaleTicketPrice = MaleTicketPrice
                WHERE FemaleTicketPrice = 0
                """);

            migrationBuilder.CreateTable(
                name: "EventDiscountCodes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatingEventId = table.Column<long>(type: "bigint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GenderScope = table.Column<int>(type: "int", nullable: false),
                    DiscountType = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StartsAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndsAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaxUsageCount = table.Column<int>(type: "int", nullable: false),
                    UsedCount = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastUsedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventDiscountCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventDiscountCodes_DatingEvents_DatingEventId",
                        column: x => x.DatingEventId,
                        principalTable: "DatingEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventDiscountCodes_DatingEventId_Code",
                table: "EventDiscountCodes",
                columns: new[] { "DatingEventId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventDiscountCodes_IsActive_StartsAtUtc_EndsAtUtc",
                table: "EventDiscountCodes",
                columns: new[] { "IsActive", "StartsAtUtc", "EndsAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventDiscountCodes");

            migrationBuilder.DropColumn(
                name: "FemaleTicketPrice",
                table: "DatingEvents");

            migrationBuilder.RenameColumn(
                name: "MaleTicketPrice",
                table: "DatingEvents",
                newName: "TicketPrice");
        }
    }
}
