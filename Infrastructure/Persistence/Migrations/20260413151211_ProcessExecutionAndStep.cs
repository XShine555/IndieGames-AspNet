using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ProcessExecutionAndStep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessExecution",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    CorrelationId = table.Column<Guid>(type: "char(36)", nullable: true),
                    ConversationId = table.Column<Guid>(type: "char(36)", nullable: true),
                    MessageId = table.Column<Guid>(type: "char(36)", nullable: true),
                    ProcessName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FinishedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessExecution", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProcessStepExecution",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ProcessExecutionId = table.Column<Guid>(type: "char(36)", nullable: false),
                    StepName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    ComponentType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Attempt = table.Column<int>(type: "int", nullable: false),
                    StartedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FinishedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessStepExecution", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessStepExecution_ProcessExecution_ProcessExecutionId",
                        column: x => x.ProcessExecutionId,
                        principalTable: "ProcessExecution",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 15, 12, 11, 307, DateTimeKind.Utc).AddTicks(9673), new DateTime(2026, 4, 13, 15, 12, 11, 307, DateTimeKind.Utc).AddTicks(9675) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 15, 12, 11, 308, DateTimeKind.Utc).AddTicks(265), new DateTime(2026, 4, 13, 15, 12, 11, 308, DateTimeKind.Utc).AddTicks(265) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 15, 12, 11, 308, DateTimeKind.Utc).AddTicks(269), new DateTime(2026, 4, 13, 15, 12, 11, 308, DateTimeKind.Utc).AddTicks(270) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 15, 12, 11, 308, DateTimeKind.Utc).AddTicks(271), new DateTime(2026, 4, 13, 15, 12, 11, 308, DateTimeKind.Utc).AddTicks(271) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 15, 12, 11, 308, DateTimeKind.Utc).AddTicks(272), new DateTime(2026, 4, 13, 15, 12, 11, 308, DateTimeKind.Utc).AddTicks(272) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 15, 12, 11, 308, DateTimeKind.Utc).AddTicks(273), new DateTime(2026, 4, 13, 15, 12, 11, 308, DateTimeKind.Utc).AddTicks(273) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 15, 12, 11, 308, DateTimeKind.Utc).AddTicks(274), new DateTime(2026, 4, 13, 15, 12, 11, 308, DateTimeKind.Utc).AddTicks(274) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 15, 12, 11, 308, DateTimeKind.Utc).AddTicks(275), new DateTime(2026, 4, 13, 15, 12, 11, 308, DateTimeKind.Utc).AddTicks(275) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 15, 12, 11, 308, DateTimeKind.Utc).AddTicks(276), new DateTime(2026, 4, 13, 15, 12, 11, 308, DateTimeKind.Utc).AddTicks(276) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 15, 12, 11, 308, DateTimeKind.Utc).AddTicks(277), new DateTime(2026, 4, 13, 15, 12, 11, 308, DateTimeKind.Utc).AddTicks(277) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 15, 12, 11, 308, DateTimeKind.Utc).AddTicks(278), new DateTime(2026, 4, 13, 15, 12, 11, 308, DateTimeKind.Utc).AddTicks(278) });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStepExecution_ProcessExecutionId",
                table: "ProcessStepExecution",
                column: "ProcessExecutionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessStepExecution");

            migrationBuilder.DropTable(
                name: "ProcessExecution");

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
    }
}
