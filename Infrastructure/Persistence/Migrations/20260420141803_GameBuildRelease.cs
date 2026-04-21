using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class GameBuildRelease : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReleaseGameBuildId",
                table: "Games",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Game_Build_Files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameBuildId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileRelativePath = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FileContentType = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Hash = table.Column<string>(type: "text", nullable: false),
                    HashAlgorithm = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game_Build_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Game_Build_Files_Game_Builds_GameBuildId",
                        column: x => x.GameBuildId,
                        principalTable: "Game_Builds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_ReleaseGameBuildId",
                table: "Games",
                column: "ReleaseGameBuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Game_Build_Files_GameBuildId",
                table: "Game_Build_Files",
                column: "GameBuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Game_Builds_ReleaseGameBuildId",
                table: "Games",
                column: "ReleaseGameBuildId",
                principalTable: "Game_Builds",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Game_Builds_ReleaseGameBuildId",
                table: "Games");

            migrationBuilder.DropTable(
                name: "Game_Build_Files");

            migrationBuilder.DropIndex(
                name: "IX_Games_ReleaseGameBuildId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "ReleaseGameBuildId",
                table: "Games");
        }
    }
}
