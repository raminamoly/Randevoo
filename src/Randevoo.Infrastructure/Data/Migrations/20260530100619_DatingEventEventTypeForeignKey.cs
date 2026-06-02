using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class DatingEventEventTypeForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EventTypeId",
                table: "DatingEvents",
                type: "bigint",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE de
                SET EventTypeId = et.Id
                FROM DatingEvents de
                INNER JOIN EventTypes et ON et.Name = de.EventType;

                UPDATE DatingEvents
                SET EventTypeId = (SELECT TOP 1 Id FROM EventTypes ORDER BY Id)
                WHERE EventTypeId IS NULL;
                """);

            migrationBuilder.AlterColumn<long>(
                name: "EventTypeId",
                table: "DatingEvents",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "EventType",
                table: "DatingEvents");

            migrationBuilder.CreateIndex(
                name: "IX_DatingEvents_EventTypeId",
                table: "DatingEvents",
                column: "EventTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DatingEvents_EventTypes_EventTypeId",
                table: "DatingEvents",
                column: "EventTypeId",
                principalTable: "EventTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatingEvents_EventTypes_EventTypeId",
                table: "DatingEvents");

            migrationBuilder.DropIndex(
                name: "IX_DatingEvents_EventTypeId",
                table: "DatingEvents");

            migrationBuilder.AddColumn<string>(
                name: "EventType",
                table: "DatingEvents",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE de
                SET EventType = et.Name
                FROM DatingEvents de
                INNER JOIN EventTypes et ON et.Id = de.EventTypeId;
                """);

            migrationBuilder.DropColumn(
                name: "EventTypeId",
                table: "DatingEvents");
        }
    }
}
