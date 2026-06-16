using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Randevoo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEndUserStageOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProfileStatus",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "InterestTagMappings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InterestId = table.Column<long>(type: "bigint", nullable: false),
                    TagId = table.Column<long>(type: "bigint", nullable: false),
                    RelevanceWeight = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestTagMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterestTagMappings_Interests_InterestId",
                        column: x => x.InterestId,
                        principalTable: "Interests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterestTagMappings_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFacingEventStatuses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatingEventId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ParticipantProfilesOpenAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LikeWindowOpenAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LikeWindowCloseAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastEvaluatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFacingEventStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFacingEventStatuses_DatingEvents_DatingEventId",
                        column: x => x.DatingEventId,
                        principalTable: "DatingEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_ProfileStatus",
                table: "UserProfiles",
                column: "ProfileStatus");

            migrationBuilder.CreateIndex(
                name: "IX_InterestTagMappings_InterestId_TagId",
                table: "InterestTagMappings",
                columns: new[] { "InterestId", "TagId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterestTagMappings_IsActive_RelevanceWeight",
                table: "InterestTagMappings",
                columns: new[] { "IsActive", "RelevanceWeight" });

            migrationBuilder.CreateIndex(
                name: "IX_InterestTagMappings_TagId",
                table: "InterestTagMappings",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFacingEventStatuses_DatingEventId",
                table: "UserFacingEventStatuses",
                column: "DatingEventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserFacingEventStatuses_Status_LastEvaluatedAtUtc",
                table: "UserFacingEventStatuses",
                columns: new[] { "Status", "LastEvaluatedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterestTagMappings");

            migrationBuilder.DropTable(
                name: "UserFacingEventStatuses");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_ProfileStatus",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "ProfileStatus",
                table: "UserProfiles");
        }
    }
}
