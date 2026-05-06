using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedExecutableFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ManifestRelativePath",
                table: "Game_Builds",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ManifestFileName",
                table: "Game_Builds",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ManifestContentType",
                table: "Game_Builds",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "ExecutableContentType",
                table: "Game_Builds",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExecutableFileName",
                table: "Game_Builds",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExecutableRelativePath",
                table: "Game_Builds",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExecutableContentType",
                table: "Game_Builds");

            migrationBuilder.DropColumn(
                name: "ExecutableFileName",
                table: "Game_Builds");

            migrationBuilder.DropColumn(
                name: "ExecutableRelativePath",
                table: "Game_Builds");

            migrationBuilder.AlterColumn<string>(
                name: "ManifestRelativePath",
                table: "Game_Builds",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ManifestFileName",
                table: "Game_Builds",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ManifestContentType",
                table: "Game_Builds",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
