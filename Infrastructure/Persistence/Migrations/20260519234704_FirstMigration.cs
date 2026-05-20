using System;
using Domain.Entities;
using Domain.Games.Enums;
using Domain.JobTracking;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:game_artwork_processing_status", "pending,processing,completed,failed")
                .Annotation("Npgsql:Enum:game_artwork_type", "capsule,header,main")
                .Annotation("Npgsql:Enum:game_build_status", "uploading_files,pending_for_processing,processing,removing,completed,failed")
                .Annotation("Npgsql:Enum:game_picture_processing_status", "pending,processing,completed,failed")
                .Annotation("Npgsql:Enum:job_tracking_status", "running,succeeded,failed,compensated")
                .Annotation("Npgsql:Enum:job_tracking_type", "consumer,activity");

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Job_Tracking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: true),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: true),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: true),
                    JobName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Status = table.Column<JobTrackingStatus>(type: "job_tracking_status", nullable: false),
                    StartedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinishedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job_Tracking", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    IdentityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DisplayUsername = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    NormalizedDisplayUsername = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.IdentityId);
                });

            migrationBuilder.CreateTable(
                name: "Job_Tracking_Step",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    JobTrackingId = table.Column<Guid>(type: "uuid", nullable: false),
                    StepName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ComponentType = table.Column<JobTrackingType>(type: "job_tracking_type", nullable: false),
                    Status = table.Column<JobTrackingStatus>(type: "job_tracking_status", nullable: false),
                    Attempt = table.Column<int>(type: "integer", nullable: false),
                    StartedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinishedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job_Tracking_Step", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Job_Tracking_Step_Job_Tracking_JobTrackingId",
                        column: x => x.JobTrackingId,
                        principalTable: "Job_Tracking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User_Game_Collections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Game_Collections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Game_Collections_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "IdentityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User_Profile_Pictures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    OriginalRelativePath = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    OriginalFileExtension = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    SmallRelativePath = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    SmallName = table.Column<string>(type: "character varying(48)", maxLength: 48, nullable: false),
                    SmallFileExtension = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    MediumRelativePath = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    MediumName = table.Column<string>(type: "character varying(48)", maxLength: 48, nullable: false),
                    MediumFileExtension = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    LargeRelativePath = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    LargeName = table.Column<string>(type: "character varying(48)", maxLength: 48, nullable: false),
                    LargeFileExtension = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "Achievement_Pictures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AchievementId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    OriginalRelativePath = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    OriginalContentType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SmallRelativePath = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    SmallName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    SmallFileContentType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    MediumRelativePath = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    MediumName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    MediumFileContentType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    LargeRelativePath = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    LargeName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    LargeContentType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ProcessingStatus = table.Column<int>(type: "integer", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievement_Pictures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User_Achievements",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AchievementId = table.Column<Guid>(type: "uuid", nullable: false),
                    UnlockedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Achievements", x => new { x.UserId, x.AchievementId });
                    table.ForeignKey(
                        name: "FK_User_Achievements_Achievements_AchievementId",
                        column: x => x.AchievementId,
                        principalTable: "Achievements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_Achievements_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "IdentityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Game_Artworks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<GameArtworkType>(type: "game_artwork_type", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    OriginalRelativePath = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    OriginalExtension = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    SmallRelativePath = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    SmallFileName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    SmallContentType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    SmallFileSizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    MediumRelativePath = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    MediumFileName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    MediumContentType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    MediumFileSizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    LargeRelativePath = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    LargeFileName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    LargeContentType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    LargeFileSizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    ProcessingStatus = table.Column<GameArtworkProcessingStatus>(type: "game_artwork_processing_status", nullable: false),
                    ProcessingError = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game_Artworks", x => x.Id);
                });

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
                });

            migrationBuilder.CreateTable(
                name: "Game_Builds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    VersionName = table.Column<string>(type: "text", nullable: false),
                    ManifestRelativePath = table.Column<string>(type: "text", nullable: true),
                    ManifestFileName = table.Column<string>(type: "text", nullable: true),
                    ManifestContentType = table.Column<string>(type: "text", nullable: true),
                    ExecutableRelativePath = table.Column<string>(type: "text", nullable: true),
                    ExecutableFileName = table.Column<string>(type: "text", nullable: true),
                    ExecutableContentType = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<GameBuildStatus>(type: "game_build_status", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game_Builds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    NormalizedTitle = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    ReleaseGameBuildId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Game_Builds_ReleaseGameBuildId",
                        column: x => x.ReleaseGameBuildId,
                        principalTable: "Game_Builds",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Games_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "IdentityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Game_Genres",
                columns: table => new
                {
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    GenreId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game_Genres", x => new { x.GameId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_Game_Genres_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Game_Genres_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Game_Store_Pictures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    OriginalRelativePath = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    OriginalContentType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    SmallRelativePath = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    SmallName = table.Column<string>(type: "character varying(48)", maxLength: 48, nullable: true),
                    SmallFileContentType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    MediumRelativePath = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    MediumName = table.Column<string>(type: "character varying(48)", maxLength: 48, nullable: true),
                    MediumFileContentType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    LargeRelativePath = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    LargeName = table.Column<string>(type: "character varying(48)", maxLength: 48, nullable: true),
                    LargeContentType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    ProcessingStatus = table.Column<GamePictureProcessingStatus>(type: "game_picture_processing_status", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "User_Cart_Items",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Cart_Items", x => new { x.UserId, x.GameId });
                    table.ForeignKey(
                        name: "FK_User_Cart_Items_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_Cart_Items_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "IdentityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User_Game_Collection_Items",
                columns: table => new
                {
                    CollectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Game_Collection_Items", x => new { x.CollectionId, x.GameId });
                    table.ForeignKey(
                        name: "FK_User_Game_Collection_Items_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_Game_Collection_Items_User_Game_Collections_Collection~",
                        column: x => x.CollectionId,
                        principalTable: "User_Game_Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User_Owned_Games",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    purchasedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                });

            migrationBuilder.CreateIndex(
                name: "IX_Achievement_Pictures_AchievementId",
                table: "Achievement_Pictures",
                column: "AchievementId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_GameId_NormalizedName",
                table: "Achievements",
                columns: new[] { "GameId", "NormalizedName" },
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
                name: "IX_Game_Build_Files_GameBuildId",
                table: "Game_Build_Files",
                column: "GameBuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Game_Builds_GameId",
                table: "Game_Builds",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Game_Genres_GenreId",
                table: "Game_Genres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Game_Store_Pictures_GameId",
                table: "Game_Store_Pictures",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_OwnerId",
                table: "Games",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_ReleaseGameBuildId",
                table: "Games",
                column: "ReleaseGameBuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Tracking_Step_JobTrackingId",
                table: "Job_Tracking_Step",
                column: "JobTrackingId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Achievements_AchievementId",
                table: "User_Achievements",
                column: "AchievementId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Cart_Items_GameId",
                table: "User_Cart_Items",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Game_Collection_Items_GameId",
                table: "User_Game_Collection_Items",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Game_Collections_UserId_NormalizedName",
                table: "User_Game_Collections",
                columns: new[] { "UserId", "NormalizedName" },
                unique: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Achievement_Pictures_Achievements_AchievementId",
                table: "Achievement_Pictures",
                column: "AchievementId",
                principalTable: "Achievements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Achievements_Games_GameId",
                table: "Achievements",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Artworks_Games_GameId",
                table: "Game_Artworks",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Build_Files_Game_Builds_GameBuildId",
                table: "Game_Build_Files",
                column: "GameBuildId",
                principalTable: "Game_Builds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Builds_Games_GameId",
                table: "Game_Builds",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_Builds_Games_GameId",
                table: "Game_Builds");

            migrationBuilder.DropTable(
                name: "Achievement_Pictures");

            migrationBuilder.DropTable(
                name: "Game_Artworks");

            migrationBuilder.DropTable(
                name: "Game_Build_Files");

            migrationBuilder.DropTable(
                name: "Game_Genres");

            migrationBuilder.DropTable(
                name: "Game_Store_Pictures");

            migrationBuilder.DropTable(
                name: "Job_Tracking_Step");

            migrationBuilder.DropTable(
                name: "User_Achievements");

            migrationBuilder.DropTable(
                name: "User_Cart_Items");

            migrationBuilder.DropTable(
                name: "User_Game_Collection_Items");

            migrationBuilder.DropTable(
                name: "User_Owned_Games");

            migrationBuilder.DropTable(
                name: "User_Profile_Pictures");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Job_Tracking");

            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "User_Game_Collections");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Game_Builds");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
