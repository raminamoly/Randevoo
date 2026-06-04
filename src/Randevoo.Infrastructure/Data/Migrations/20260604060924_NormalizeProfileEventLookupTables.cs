using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeProfileEventLookupTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BirthMonth",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "CityId",
                table: "UserProfiles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CountryId",
                table: "UserProfiles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EducationLevelId",
                table: "UserProfiles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "GenderId",
                table: "UserProfiles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZodiacSign",
                table: "UserProfiles",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "CityId",
                table: "DatingEvents",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CountryId",
                table: "DatingEvents",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MinimumEducationLevelId",
                table: "DatingEvents",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EducationLevels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genders", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "EducationLevels",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "DisplayOrder", "IsActive", "IsDeleted", "Rank", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 0, true, false, 0, "ثبت نشده", null },
                    { 2L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, false, 1, "دیپلم", null },
                    { 3L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, false, 2, "لیسانس", null },
                    { 4L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, false, 3, "فوق لیسانس", null },
                    { 5L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 4, true, false, 4, "دکترای حرفه ای / PHD / پزشک / دندان پزشک", null }
                });

            migrationBuilder.InsertData(
                table: "Genders",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "DisplayOrder", "IsActive", "IsDeleted", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 0, true, false, "نامشخص", null },
                    { 2L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, false, "آقا", null },
                    { 3L, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, false, "خانم", null }
                });

            migrationBuilder.Sql("""
                UPDATE UserProfiles
                SET BirthMonth = MONTH(DateOfBirth),
                    ZodiacSign = CASE
                        WHEN (MONTH(DateOfBirth) = 3 AND DAY(DateOfBirth) >= 21) OR (MONTH(DateOfBirth) = 4 AND DAY(DateOfBirth) <= 19) THEN 'Aries'
                        WHEN (MONTH(DateOfBirth) = 4 AND DAY(DateOfBirth) >= 20) OR (MONTH(DateOfBirth) = 5 AND DAY(DateOfBirth) <= 20) THEN 'Taurus'
                        WHEN (MONTH(DateOfBirth) = 5 AND DAY(DateOfBirth) >= 21) OR (MONTH(DateOfBirth) = 6 AND DAY(DateOfBirth) <= 20) THEN 'Gemini'
                        WHEN (MONTH(DateOfBirth) = 6 AND DAY(DateOfBirth) >= 21) OR (MONTH(DateOfBirth) = 7 AND DAY(DateOfBirth) <= 22) THEN 'Cancer'
                        WHEN (MONTH(DateOfBirth) = 7 AND DAY(DateOfBirth) >= 23) OR (MONTH(DateOfBirth) = 8 AND DAY(DateOfBirth) <= 22) THEN 'Leo'
                        WHEN (MONTH(DateOfBirth) = 8 AND DAY(DateOfBirth) >= 23) OR (MONTH(DateOfBirth) = 9 AND DAY(DateOfBirth) <= 22) THEN 'Virgo'
                        WHEN (MONTH(DateOfBirth) = 9 AND DAY(DateOfBirth) >= 23) OR (MONTH(DateOfBirth) = 10 AND DAY(DateOfBirth) <= 22) THEN 'Libra'
                        WHEN (MONTH(DateOfBirth) = 10 AND DAY(DateOfBirth) >= 23) OR (MONTH(DateOfBirth) = 11 AND DAY(DateOfBirth) <= 21) THEN 'Scorpio'
                        WHEN (MONTH(DateOfBirth) = 11 AND DAY(DateOfBirth) >= 22) OR (MONTH(DateOfBirth) = 12 AND DAY(DateOfBirth) <= 21) THEN 'Sagittarius'
                        WHEN (MONTH(DateOfBirth) = 12 AND DAY(DateOfBirth) >= 22) OR (MONTH(DateOfBirth) = 1 AND DAY(DateOfBirth) <= 19) THEN 'Capricorn'
                        WHEN (MONTH(DateOfBirth) = 1 AND DAY(DateOfBirth) >= 20) OR (MONTH(DateOfBirth) = 2 AND DAY(DateOfBirth) <= 18) THEN 'Aquarius'
                        ELSE 'Pisces'
                    END,
                    GenderId = CASE Gender WHEN 1 THEN 2 WHEN 2 THEN 3 ELSE 1 END,
                    EducationLevelId = CASE
                        WHEN EducationLevel = 1 THEN 2
                        WHEN EducationLevel IN (2, 3) THEN 3
                        WHEN EducationLevel = 4 THEN 4
                        WHEN EducationLevel IN (5, 6) THEN 5
                        ELSE 1
                    END;
                """);

            migrationBuilder.Sql("""
                UPDATE up
                SET CountryId = c.Id,
                    CityId = ci.Id
                FROM UserProfiles up
                INNER JOIN Countries c ON c.Name = up.Location_Country
                INNER JOIN Cities ci ON ci.CountryId = c.Id AND ci.Name = up.Location_City;
                """);

            migrationBuilder.Sql("""
                UPDATE de
                SET CountryId = c.Id,
                    CityId = ci.Id,
                    MinimumEducationLevelId = CASE
                        WHEN de.EducationLevelRestriction = 1 THEN 2
                        WHEN de.EducationLevelRestriction = 2 THEN 3
                        WHEN de.EducationLevelRestriction = 3 THEN 4
                        WHEN de.EducationLevelRestriction = 4 THEN 5
                        ELSE NULL
                    END
                FROM DatingEvents de
                LEFT JOIN Countries c ON c.Name = de.Location_Country
                LEFT JOIN Cities ci ON ci.CountryId = c.Id AND ci.Name = de.Location_City;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_CityId",
                table: "UserProfiles",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_CountryId",
                table: "UserProfiles",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_EducationLevelId",
                table: "UserProfiles",
                column: "EducationLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_GenderId",
                table: "UserProfiles",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_DatingEvents_CityId",
                table: "DatingEvents",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_DatingEvents_CountryId",
                table: "DatingEvents",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_DatingEvents_MinimumEducationLevelId",
                table: "DatingEvents",
                column: "MinimumEducationLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationLevels_Title",
                table: "EducationLevels",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Genders_Title",
                table: "Genders",
                column: "Title",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DatingEvents_Cities_CityId",
                table: "DatingEvents",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DatingEvents_Countries_CountryId",
                table: "DatingEvents",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DatingEvents_EducationLevels_MinimumEducationLevelId",
                table: "DatingEvents",
                column: "MinimumEducationLevelId",
                principalTable: "EducationLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Cities_CityId",
                table: "UserProfiles",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Countries_CountryId",
                table: "UserProfiles",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_EducationLevels_EducationLevelId",
                table: "UserProfiles",
                column: "EducationLevelId",
                principalTable: "EducationLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Genders_GenderId",
                table: "UserProfiles",
                column: "GenderId",
                principalTable: "Genders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatingEvents_Cities_CityId",
                table: "DatingEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_DatingEvents_Countries_CountryId",
                table: "DatingEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_DatingEvents_EducationLevels_MinimumEducationLevelId",
                table: "DatingEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Cities_CityId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Countries_CountryId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_EducationLevels_EducationLevelId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Genders_GenderId",
                table: "UserProfiles");

            migrationBuilder.DropTable(
                name: "EducationLevels");

            migrationBuilder.DropTable(
                name: "Genders");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_CityId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_CountryId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_EducationLevelId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_GenderId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_DatingEvents_CityId",
                table: "DatingEvents");

            migrationBuilder.DropIndex(
                name: "IX_DatingEvents_CountryId",
                table: "DatingEvents");

            migrationBuilder.DropIndex(
                name: "IX_DatingEvents_MinimumEducationLevelId",
                table: "DatingEvents");

            migrationBuilder.DropColumn(
                name: "BirthMonth",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "EducationLevelId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "GenderId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "ZodiacSign",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "DatingEvents");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "DatingEvents");

            migrationBuilder.DropColumn(
                name: "MinimumEducationLevelId",
                table: "DatingEvents");
        }
    }
}
