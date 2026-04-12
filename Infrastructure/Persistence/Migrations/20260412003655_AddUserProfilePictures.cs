using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfilePictures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_Original_Pictures_Games_GameId",
                table: "Game_Original_Pictures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Game_Original_Pictures",
                table: "Game_Original_Pictures");

            migrationBuilder.RenameTable(
                name: "Game_Original_Pictures",
                newName: "Game_Store_Pictures");

            migrationBuilder.RenameIndex(
                name: "IX_Game_Original_Pictures_GameId",
                table: "Game_Store_Pictures",
                newName: "IX_Game_Store_Pictures_GameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Game_Store_Pictures",
                table: "Game_Store_Pictures",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Game_Artworks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Kind = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    OriginalRelativePath = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    OriginalFileName = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    OriginalFormat = table.Column<int>(type: "int", nullable: false),
                    ProcessingStatus = table.Column<int>(type: "int", nullable: false),
                    ProcessingError = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true),
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
                name: "Game_Artwork_Variants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    GameArtworkId = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<int>(type: "int", nullable: false),
                    Format = table.Column<int>(type: "int", nullable: false),
                    RelativePath = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    FileName = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    Width = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    FileSizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    IsPrimary = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game_Artwork_Variants", x => x.Id);
                    table.CheckConstraint("CK_GameArtworkVariant_FileSizeInBytes", "FileSizeInBytes > 0");
                    table.CheckConstraint("CK_GameArtworkVariant_Height", "Height > 0");
                    table.CheckConstraint("CK_GameArtworkVariant_Width", "Width > 0");
                    table.ForeignKey(
                        name: "FK_Game_Artwork_Variants_Game_Artworks_GameArtworkId",
                        column: x => x.GameArtworkId,
                        principalTable: "Game_Artworks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Game_Artwork_Variants_GameArtworkId_IsPrimary",
                table: "Game_Artwork_Variants",
                columns: new[] { "GameArtworkId", "IsPrimary" });

            migrationBuilder.CreateIndex(
                name: "IX_Game_Artwork_Variants_GameArtworkId_Size_Format",
                table: "Game_Artwork_Variants",
                columns: new[] { "GameArtworkId", "Size", "Format" },
                unique: true);

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
                name: "IX_User_Profile_Pictures_UserId",
                table: "User_Profile_Pictures",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Store_Pictures_Games_GameId",
                table: "Game_Store_Pictures",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_Store_Pictures_Games_GameId",
                table: "Game_Store_Pictures");

            migrationBuilder.DropTable(
                name: "Game_Artwork_Variants");

            migrationBuilder.DropTable(
                name: "User_Profile_Pictures");

            migrationBuilder.DropTable(
                name: "Game_Artworks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Game_Store_Pictures",
                table: "Game_Store_Pictures");

            migrationBuilder.RenameTable(
                name: "Game_Store_Pictures",
                newName: "Game_Original_Pictures");

            migrationBuilder.RenameIndex(
                name: "IX_Game_Store_Pictures_GameId",
                table: "Game_Original_Pictures",
                newName: "IX_Game_Original_Pictures_GameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Game_Original_Pictures",
                table: "Game_Original_Pictures",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Original_Pictures_Games_GameId",
                table: "Game_Original_Pictures",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
