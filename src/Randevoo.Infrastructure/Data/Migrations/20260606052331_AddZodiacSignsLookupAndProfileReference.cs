using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddZodiacSignsLookupAndProfileReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ZodiacSignId",
                table: "UserProfiles",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ZodiacSigns",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZodiacSigns", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ZodiacSigns",
                columns: new[] { "Id", "Code", "CreatedAt", "DeletedAt", "DisplayOrder", "IsActive", "IsDeleted", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, "Aries", new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, false, "حمل", null },
                    { 2L, "Taurus", new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, false, "ثور", null },
                    { 3L, "Gemini", new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, false, "جوزا", null },
                    { 4L, "Cancer", new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, 4, true, false, "سرطان", null },
                    { 5L, "Leo", new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, 5, true, false, "اسد", null },
                    { 6L, "Virgo", new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, 6, true, false, "سنبله", null },
                    { 7L, "Libra", new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, 7, true, false, "میزان", null },
                    { 8L, "Scorpio", new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, 8, true, false, "عقرب", null },
                    { 9L, "Sagittarius", new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, 9, true, false, "قوس", null },
                    { 10L, "Capricorn", new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, 10, true, false, "جدی", null },
                    { 11L, "Aquarius", new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, 11, true, false, "دلو", null },
                    { 12L, "Pisces", new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, 12, true, false, "حوت", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_ZodiacSignId",
                table: "UserProfiles",
                column: "ZodiacSignId");

            migrationBuilder.CreateIndex(
                name: "IX_ZodiacSigns_Code",
                table: "ZodiacSigns",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ZodiacSigns_Title",
                table: "ZodiacSigns",
                column: "Title",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_ZodiacSigns_ZodiacSignId",
                table: "UserProfiles",
                column: "ZodiacSignId",
                principalTable: "ZodiacSigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_ZodiacSigns_ZodiacSignId",
                table: "UserProfiles");

            migrationBuilder.DropTable(
                name: "ZodiacSigns");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_ZodiacSignId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "ZodiacSignId",
                table: "UserProfiles");
        }
    }
}
