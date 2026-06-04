using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryCityLookups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Code", "CreatedAt", "DeletedAt", "DisplayOrder", "IsActive", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, "IR", new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, false, "ایران", null },
                    { 2L, "AE", new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, false, "امارات متحده عربی", null },
                    { 3L, "TR", new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, false, "ترکیه", null }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "CountryId", "CreatedAt", "DeletedAt", "DisplayOrder", "IsActive", "IsDeleted", "Latitude", "Longitude", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, 1L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, false, 35.689200m, 51.389000m, "تهران", null },
                    { 2L, 1L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, false, 36.260500m, 59.616800m, "مشهد", null },
                    { 3L, 1L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, false, 29.591800m, 52.583700m, "شیراز", null },
                    { 4L, 1L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 4, true, false, 32.654600m, 51.668000m, "اصفهان", null },
                    { 5L, 1L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 5, true, false, 38.096200m, 46.273800m, "تبریز", null },
                    { 6L, 2L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, false, 25.204800m, 55.270800m, "دبی", null },
                    { 7L, 2L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, false, 24.453900m, 54.377300m, "ابوظبی", null },
                    { 8L, 3L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, false, 41.008200m, 28.978400m, "استانبول", null },
                    { 9L, 3L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, false, 39.933400m, 32.859700m, "آنکارا", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId_Name",
                table: "Cities",
                columns: new[] { "CountryId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Code",
                table: "Countries",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Name",
                table: "Countries",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
