using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SplitEventReviewAndOperationalStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReviewStatus",
                table: "DatingEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                UPDATE DatingEvents
                SET ReviewStatus = CASE
                    WHEN IsOpenForSell = 1 THEN 2
                    WHEN IsCancelled = 1 THEN 2
                    WHEN DateTimeEnd <= SYSUTCDATETIME() THEN 2
                    ELSE 1
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewStatus",
                table: "DatingEvents");
        }
    }
}
