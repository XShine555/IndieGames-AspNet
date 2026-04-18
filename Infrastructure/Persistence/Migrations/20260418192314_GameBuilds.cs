using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class GameBuilds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:game_artwork_processing_status", "pending,processing,completed,failed")
                .Annotation("Npgsql:Enum:game_artwork_type", "capsule,header,main")
                .Annotation("Npgsql:Enum:game_build_status", "completed,in_progress,pending,failed")
                .Annotation("Npgsql:Enum:game_picture_processing_status", "pending,processing,completed,failed")
                .Annotation("Npgsql:Enum:game_store_readiness_status", "not_ready_for_store,ready_for_store")
                .Annotation("Npgsql:Enum:job_tracking_status", "running,succeeded,failed,compensated")
                .Annotation("Npgsql:Enum:job_tracking_type", "consumer,activity")
                .OldAnnotation("Npgsql:Enum:game_artwork_processing_status", "pending,processing,completed,failed")
                .OldAnnotation("Npgsql:Enum:game_artwork_type", "capsule,header,main")
                .OldAnnotation("Npgsql:Enum:game_picture_processing_status", "pending,processing,completed,failed")
                .OldAnnotation("Npgsql:Enum:game_store_readiness_status", "not_ready_for_store,ready_for_store")
                .OldAnnotation("Npgsql:Enum:job_tracking_status", "running,succeeded,failed,compensated")
                .OldAnnotation("Npgsql:Enum:job_tracking_type", "consumer,activity");

            migrationBuilder.CreateTable(
                name: "Game_Builds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    VersionName = table.Column<string>(type: "text", nullable: false),
                    manifestRelativePath = table.Column<string>(type: "text", nullable: false),
                    manifestFileName = table.Column<string>(type: "text", nullable: false),
                    manifestContentType = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game_Builds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Game_Builds_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Game_Builds_GameId",
                table: "Game_Builds",
                column: "GameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Game_Builds");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:game_artwork_processing_status", "pending,processing,completed,failed")
                .Annotation("Npgsql:Enum:game_artwork_type", "capsule,header,main")
                .Annotation("Npgsql:Enum:game_picture_processing_status", "pending,processing,completed,failed")
                .Annotation("Npgsql:Enum:game_store_readiness_status", "not_ready_for_store,ready_for_store")
                .Annotation("Npgsql:Enum:job_tracking_status", "running,succeeded,failed,compensated")
                .Annotation("Npgsql:Enum:job_tracking_type", "consumer,activity")
                .OldAnnotation("Npgsql:Enum:game_artwork_processing_status", "pending,processing,completed,failed")
                .OldAnnotation("Npgsql:Enum:game_artwork_type", "capsule,header,main")
                .OldAnnotation("Npgsql:Enum:game_build_status", "completed,in_progress,pending,failed")
                .OldAnnotation("Npgsql:Enum:game_picture_processing_status", "pending,processing,completed,failed")
                .OldAnnotation("Npgsql:Enum:game_store_readiness_status", "not_ready_for_store,ready_for_store")
                .OldAnnotation("Npgsql:Enum:job_tracking_status", "running,succeeded,failed,compensated")
                .OldAnnotation("Npgsql:Enum:job_tracking_type", "consumer,activity");
        }
    }
}
