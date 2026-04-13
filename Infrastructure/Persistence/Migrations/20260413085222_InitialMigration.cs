using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    IdentityId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    Username = table.Column<string>(type: "varchar(24)", maxLength: 24, nullable: false),
                    DisplayUsername = table.Column<string>(type: "varchar(24)", maxLength: 24, nullable: false),
                    NormalizedDisplayUsername = table.Column<string>(type: "varchar(24)", maxLength: 24, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.IdentityId);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    NormalizedTitle = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: false),
                    OwnerId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "IdentityId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "User_Profile_Pictures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    OriginalName = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true),
                    OriginalRelativePath = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true),
                    OriginalFileExtension = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: true),
                    SmallRelativePath = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    SmallName = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    SmallFileExtension = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: false),
                    MediumRelativePath = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    MediumName = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    MediumFileExtension = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: false),
                    LargeRelativePath = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    LargeName = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    LargeFileExtension = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Profile_Pictures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Profile_Pictures_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "IdentityId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Game_Artworks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    OriginalRelativePath = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    OriginalFileName = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    OriginalExtension = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: false),
                    SmallRelativePath = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    SmallFileName = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    SmallContentType = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    SmallWidth = table.Column<int>(type: "int", nullable: false),
                    SmallHeight = table.Column<int>(type: "int", nullable: false),
                    SmallFileSizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    MediumRelativePath = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    MediumFileName = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    MediumContentType = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    MediumWidth = table.Column<int>(type: "int", nullable: false),
                    MediumHeight = table.Column<int>(type: "int", nullable: false),
                    MediumFileSizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    LargeRelativePath = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    LargeFileName = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    LargeContentType = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    LargeWidth = table.Column<int>(type: "int", nullable: false),
                    LargeHeight = table.Column<int>(type: "int", nullable: false),
                    LargeFileSizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    ProcessingStatus = table.Column<int>(type: "int", nullable: false),
                    ProcessingError = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game_Artworks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Game_Artworks_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Game_Store_Pictures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    OriginalName = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    OriginalRelativePath = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    OriginalContentType = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    SmallRelativePath = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true),
                    SmallName = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true),
                    SmallFileContentType = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: true),
                    MediumRelativePath = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true),
                    MediumName = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true),
                    MediumContentType = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: true),
                    LargeRelativePath = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true),
                    LargeName = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true),
                    LargeContentType = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: true),
                    ProcessingStatus = table.Column<int>(type: "int", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game_Store_Pictures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Game_Store_Pictures_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    NormalizedName = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Genres_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "User_Owned_Games",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    purchasedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Owned_Games", x => new { x.UserId, x.GameId });
                    table.ForeignKey(
                        name: "FK_User_Owned_Games_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_Owned_Games_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "IdentityId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "CreatedAt", "GameId", "Name", "NormalizedName", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 13, 8, 52, 22, 147, DateTimeKind.Utc).AddTicks(4704), null, "Action", "ACTION", new DateTime(2026, 4, 13, 8, 52, 22, 147, DateTimeKind.Utc).AddTicks(4707) },
                    { 2, new DateTime(2026, 4, 13, 8, 52, 22, 147, DateTimeKind.Utc).AddTicks(5698), null, "Adventure", "ADVENTURE", new DateTime(2026, 4, 13, 8, 52, 22, 147, DateTimeKind.Utc).AddTicks(5699) },
                    { 3, new DateTime(2026, 4, 13, 8, 52, 22, 147, DateTimeKind.Utc).AddTicks(5704), null, "RPG", "RPG", new DateTime(2026, 4, 13, 8, 52, 22, 147, DateTimeKind.Utc).AddTicks(5704) },
                    { 4, new DateTime(2026, 4, 13, 8, 52, 22, 147, DateTimeKind.Utc).AddTicks(5706), null, "Strategy", "STRATEGY", new DateTime(2026, 4, 13, 8, 52, 22, 147, DateTimeKind.Utc).AddTicks(5706) },
                    { 5, new DateTime(2026, 4, 13, 8, 52, 22, 147, DateTimeKind.Utc).AddTicks(5707), null, "Simulation", "SIMULATION", new DateTime(2026, 4, 13, 8, 52, 22, 147, DateTimeKind.Utc).AddTicks(5708) },
                    { 6, new DateTime(2026, 4, 13, 8, 52, 22, 147, DateTimeKind.Utc).AddTicks(5708), null, "Sports", "SPORTS", new DateTime(2026, 4, 13, 8, 52, 22, 147, DateTimeKind.Utc).AddTicks(5709) },
                    { 7, new DateTime(2026, 4, 13, 8, 52, 22, 147, DateTimeKind.Utc).AddTicks(5710), null, "Puzzle", "PUZZLE", new DateTime(2026, 4, 13, 8, 52, 22, 147, DateTimeKind.Utc).AddTicks(5710) },
                    { 8, new DateTime(2026, 4, 13, 8, 52, 22, 147, DateTimeKind.Utc).AddTicks(5711), null, "Horror", "HORROR", new DateTime(2026, 4, 13, 8, 52, 22, 147, DateTimeKind.Utc).AddTicks(5711) },
                    { 9, new DateTime(2026, 4, 13, 8, 52, 22, 147, DateTimeKind.Utc).AddTicks(5712), null, "Racing", "RACING", new DateTime(2026, 4, 13, 8, 52, 22, 147, DateTimeKind.Utc).AddTicks(5712) },
                    { 10, new DateTime(2026, 4, 13, 8, 52, 22, 147, DateTimeKind.Utc).AddTicks(5713), null, "Indie", "INDIE", new DateTime(2026, 4, 13, 8, 52, 22, 147, DateTimeKind.Utc).AddTicks(5713) },
                    { 11, new DateTime(2026, 4, 13, 8, 52, 22, 147, DateTimeKind.Utc).AddTicks(5714), null, "FPS", "FPS", new DateTime(2026, 4, 13, 8, 52, 22, 147, DateTimeKind.Utc).AddTicks(5714) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Game_Artworks_GameId_Type",
                table: "Game_Artworks",
                columns: new[] { "GameId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_Game_Artworks_GameId_Type_SortOrder",
                table: "Game_Artworks",
                columns: new[] { "GameId", "Type", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Game_Store_Pictures_GameId",
                table: "Game_Store_Pictures",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_OwnerId",
                table: "Games",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Genres_GameId",
                table: "Genres",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Owned_Games_GameId",
                table: "User_Owned_Games",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Profile_Pictures_UserId",
                table: "User_Profile_Pictures",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdentityId",
                table: "Users",
                column: "IdentityId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Game_Artworks");

            migrationBuilder.DropTable(
                name: "Game_Store_Pictures");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "User_Owned_Games");

            migrationBuilder.DropTable(
                name: "User_Profile_Pictures");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
