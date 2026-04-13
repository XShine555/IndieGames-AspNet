using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class IsPublishedAndIsPublic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Games",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Games",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 15, 3, 25, 865, DateTimeKind.Utc).AddTicks(9756), new DateTime(2026, 4, 13, 15, 3, 25, 865, DateTimeKind.Utc).AddTicks(9759) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 15, 3, 25, 866, DateTimeKind.Utc).AddTicks(419), new DateTime(2026, 4, 13, 15, 3, 25, 866, DateTimeKind.Utc).AddTicks(420) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 15, 3, 25, 866, DateTimeKind.Utc).AddTicks(425), new DateTime(2026, 4, 13, 15, 3, 25, 866, DateTimeKind.Utc).AddTicks(425) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 15, 3, 25, 866, DateTimeKind.Utc).AddTicks(426), new DateTime(2026, 4, 13, 15, 3, 25, 866, DateTimeKind.Utc).AddTicks(427) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 15, 3, 25, 866, DateTimeKind.Utc).AddTicks(428), new DateTime(2026, 4, 13, 15, 3, 25, 866, DateTimeKind.Utc).AddTicks(428) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 15, 3, 25, 866, DateTimeKind.Utc).AddTicks(429), new DateTime(2026, 4, 13, 15, 3, 25, 866, DateTimeKind.Utc).AddTicks(429) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 15, 3, 25, 866, DateTimeKind.Utc).AddTicks(430), new DateTime(2026, 4, 13, 15, 3, 25, 866, DateTimeKind.Utc).AddTicks(430) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 15, 3, 25, 866, DateTimeKind.Utc).AddTicks(431), new DateTime(2026, 4, 13, 15, 3, 25, 866, DateTimeKind.Utc).AddTicks(431) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 15, 3, 25, 866, DateTimeKind.Utc).AddTicks(432), new DateTime(2026, 4, 13, 15, 3, 25, 866, DateTimeKind.Utc).AddTicks(432) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 15, 3, 25, 866, DateTimeKind.Utc).AddTicks(433), new DateTime(2026, 4, 13, 15, 3, 25, 866, DateTimeKind.Utc).AddTicks(433) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 15, 3, 25, 866, DateTimeKind.Utc).AddTicks(434), new DateTime(2026, 4, 13, 15, 3, 25, 866, DateTimeKind.Utc).AddTicks(434) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Games");

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 14, 36, 54, 871, DateTimeKind.Utc).AddTicks(4387), new DateTime(2026, 4, 13, 14, 36, 54, 871, DateTimeKind.Utc).AddTicks(4388) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 14, 36, 54, 871, DateTimeKind.Utc).AddTicks(4954), new DateTime(2026, 4, 13, 14, 36, 54, 871, DateTimeKind.Utc).AddTicks(4954) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 14, 36, 54, 871, DateTimeKind.Utc).AddTicks(4958), new DateTime(2026, 4, 13, 14, 36, 54, 871, DateTimeKind.Utc).AddTicks(4959) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 14, 36, 54, 871, DateTimeKind.Utc).AddTicks(4960), new DateTime(2026, 4, 13, 14, 36, 54, 871, DateTimeKind.Utc).AddTicks(4961) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 14, 36, 54, 871, DateTimeKind.Utc).AddTicks(4962), new DateTime(2026, 4, 13, 14, 36, 54, 871, DateTimeKind.Utc).AddTicks(4962) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 14, 36, 54, 871, DateTimeKind.Utc).AddTicks(4963), new DateTime(2026, 4, 13, 14, 36, 54, 871, DateTimeKind.Utc).AddTicks(4963) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 14, 36, 54, 871, DateTimeKind.Utc).AddTicks(4964), new DateTime(2026, 4, 13, 14, 36, 54, 871, DateTimeKind.Utc).AddTicks(4964) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 14, 36, 54, 871, DateTimeKind.Utc).AddTicks(4965), new DateTime(2026, 4, 13, 14, 36, 54, 871, DateTimeKind.Utc).AddTicks(4965) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 14, 36, 54, 871, DateTimeKind.Utc).AddTicks(4966), new DateTime(2026, 4, 13, 14, 36, 54, 871, DateTimeKind.Utc).AddTicks(4966) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 14, 36, 54, 871, DateTimeKind.Utc).AddTicks(4967), new DateTime(2026, 4, 13, 14, 36, 54, 871, DateTimeKind.Utc).AddTicks(4967) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 14, 36, 54, 871, DateTimeKind.Utc).AddTicks(4968), new DateTime(2026, 4, 13, 14, 36, 54, 871, DateTimeKind.Utc).AddTicks(4968) });
        }
    }
}
