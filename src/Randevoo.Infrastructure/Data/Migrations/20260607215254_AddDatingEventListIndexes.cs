using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDatingEventListIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DatingEvents_DateTimeStart_Id",
                table: "DatingEvents",
                columns: new[] { "DateTimeStart", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_DatingEvents_IsCancelled_DateTimeEnd",
                table: "DatingEvents",
                columns: new[] { "IsCancelled", "DateTimeEnd" });

            migrationBuilder.CreateIndex(
                name: "IX_DatingEvents_IsCancelled_IsOpenForSell_DateTimeEnd",
                table: "DatingEvents",
                columns: new[] { "IsCancelled", "IsOpenForSell", "DateTimeEnd" });

            migrationBuilder.CreateIndex(
                name: "IX_DatingEvents_ReviewStatus_DateTimeStart",
                table: "DatingEvents",
                columns: new[] { "ReviewStatus", "DateTimeStart" });

            migrationBuilder.CreateIndex(
                name: "IX_DatingEvents_UpdatedAt_Id",
                table: "DatingEvents",
                columns: new[] { "UpdatedAt", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DatingEvents_DateTimeStart_Id",
                table: "DatingEvents");

            migrationBuilder.DropIndex(
                name: "IX_DatingEvents_IsCancelled_DateTimeEnd",
                table: "DatingEvents");

            migrationBuilder.DropIndex(
                name: "IX_DatingEvents_IsCancelled_IsOpenForSell_DateTimeEnd",
                table: "DatingEvents");

            migrationBuilder.DropIndex(
                name: "IX_DatingEvents_ReviewStatus_DateTimeStart",
                table: "DatingEvents");

            migrationBuilder.DropIndex(
                name: "IX_DatingEvents_UpdatedAt_Id",
                table: "DatingEvents");
        }
    }
}
