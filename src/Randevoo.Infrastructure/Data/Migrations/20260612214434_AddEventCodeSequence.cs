using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEventCodeSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateSequence<int>(
                name: "EventCodeSequence",
                schema: "dbo",
                startValue: 1200L);

            migrationBuilder.AddColumn<int>(
                name: "EventCode",
                table: "DatingEvents",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR dbo.EventCodeSequence");

            migrationBuilder.CreateIndex(
                name: "IX_DatingEvents_EventCode",
                table: "DatingEvents",
                column: "EventCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DatingEvents_EventCode",
                table: "DatingEvents");

            migrationBuilder.DropColumn(
                name: "EventCode",
                table: "DatingEvents");

            migrationBuilder.DropSequence(
                name: "EventCodeSequence",
                schema: "dbo");
        }
    }
}
