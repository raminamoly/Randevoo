using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeLocationTagsAndRialCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location_City",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Location_Country",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Location_City",
                table: "DatingEvents");

            migrationBuilder.DropColumn(
                name: "Location_Country",
                table: "DatingEvents");

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventTags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatingEventId = table.Column<long>(type: "bigint", nullable: false),
                    TagId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventTags_DatingEvents_DatingEventId",
                        column: x => x.DatingEventId,
                        principalTable: "DatingEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IsActive", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, "شب اجتماعی", null },
                    { 2L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, "شام", null },
                    { 3L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, "کافه", null },
                    { 4L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, "بازی", null },
                    { 5L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, "هنر", null },
                    { 6L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, "کارگاه", null },
                    { 7L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, "موسیقی", null },
                    { 8L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, "روف تاپ", null }
                });

            migrationBuilder.Sql("""
                INSERT INTO Tags (Name, IsActive, CreatedAt, IsDeleted)
                SELECT DISTINCT TRIM(value), 1, SYSUTCDATETIME(), 0
                FROM DatingEvents
                CROSS APPLY STRING_SPLIT(EventTagsSerialized, '|')
                WHERE TRIM(value) <> ''
                  AND NOT EXISTS (
                      SELECT 1
                      FROM Tags
                      WHERE Tags.Name = TRIM(value)
                  );
                """);

            migrationBuilder.Sql("""
                INSERT INTO EventTags (DatingEventId, TagId, CreatedAt, IsDeleted)
                SELECT DISTINCT de.Id, tag.Id, SYSUTCDATETIME(), 0
                FROM DatingEvents de
                CROSS APPLY STRING_SPLIT(de.EventTagsSerialized, '|') splitTag
                INNER JOIN Tags tag ON tag.Name = TRIM(splitTag.value)
                WHERE TRIM(splitTag.value) <> '';
                """);

            migrationBuilder.DropColumn(
                name: "EventTagsSerialized",
                table: "DatingEvents");

            migrationBuilder.CreateIndex(
                name: "IX_EventTags_DatingEventId_TagId",
                table: "EventTags",
                columns: new[] { "DatingEventId", "TagId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventTags_TagId",
                table: "EventTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventTags");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.AddColumn<string>(
                name: "Location_City",
                table: "UserProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Location_Country",
                table: "UserProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EventTagsSerialized",
                table: "DatingEvents",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Location_City",
                table: "DatingEvents",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Location_Country",
                table: "DatingEvents",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
