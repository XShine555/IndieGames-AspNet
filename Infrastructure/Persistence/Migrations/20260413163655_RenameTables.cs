using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessStepExecution_ProcessExecution_ProcessExecutionId",
                table: "ProcessStepExecution");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProcessStepExecution",
                table: "ProcessStepExecution");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProcessExecution",
                table: "ProcessExecution");

            migrationBuilder.RenameTable(
                name: "ProcessStepExecution",
                newName: "Process_Step_Execution");

            migrationBuilder.RenameTable(
                name: "ProcessExecution",
                newName: "Process_Execution");

            migrationBuilder.RenameIndex(
                name: "IX_ProcessStepExecution_ProcessExecutionId",
                table: "Process_Step_Execution",
                newName: "IX_Process_Step_Execution_ProcessExecutionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Process_Step_Execution",
                table: "Process_Step_Execution",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Process_Execution",
                table: "Process_Execution",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 16, 36, 55, 300, DateTimeKind.Utc).AddTicks(8050), new DateTime(2026, 4, 13, 16, 36, 55, 300, DateTimeKind.Utc).AddTicks(8053) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 16, 36, 55, 300, DateTimeKind.Utc).AddTicks(8728), new DateTime(2026, 4, 13, 16, 36, 55, 300, DateTimeKind.Utc).AddTicks(8728) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 16, 36, 55, 300, DateTimeKind.Utc).AddTicks(8731), new DateTime(2026, 4, 13, 16, 36, 55, 300, DateTimeKind.Utc).AddTicks(8731) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 16, 36, 55, 300, DateTimeKind.Utc).AddTicks(8733), new DateTime(2026, 4, 13, 16, 36, 55, 300, DateTimeKind.Utc).AddTicks(8733) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 16, 36, 55, 300, DateTimeKind.Utc).AddTicks(8734), new DateTime(2026, 4, 13, 16, 36, 55, 300, DateTimeKind.Utc).AddTicks(8734) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 16, 36, 55, 300, DateTimeKind.Utc).AddTicks(8735), new DateTime(2026, 4, 13, 16, 36, 55, 300, DateTimeKind.Utc).AddTicks(8736) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 16, 36, 55, 300, DateTimeKind.Utc).AddTicks(8737), new DateTime(2026, 4, 13, 16, 36, 55, 300, DateTimeKind.Utc).AddTicks(8737) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 16, 36, 55, 300, DateTimeKind.Utc).AddTicks(8738), new DateTime(2026, 4, 13, 16, 36, 55, 300, DateTimeKind.Utc).AddTicks(8739) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 16, 36, 55, 300, DateTimeKind.Utc).AddTicks(8739), new DateTime(2026, 4, 13, 16, 36, 55, 300, DateTimeKind.Utc).AddTicks(8740) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 16, 36, 55, 300, DateTimeKind.Utc).AddTicks(8741), new DateTime(2026, 4, 13, 16, 36, 55, 300, DateTimeKind.Utc).AddTicks(8741) });

            migrationBuilder.UpdateData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 13, 16, 36, 55, 300, DateTimeKind.Utc).AddTicks(8742), new DateTime(2026, 4, 13, 16, 36, 55, 300, DateTimeKind.Utc).AddTicks(8743) });

            migrationBuilder.AddForeignKey(
                name: "FK_Process_Step_Execution_Process_Execution_ProcessExecutionId",
                table: "Process_Step_Execution",
                column: "ProcessExecutionId",
                principalTable: "Process_Execution",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Process_Step_Execution_Process_Execution_ProcessExecutionId",
                table: "Process_Step_Execution");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Process_Step_Execution",
                table: "Process_Step_Execution");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Process_Execution",
                table: "Process_Execution");

            migrationBuilder.RenameTable(
                name: "Process_Step_Execution",
                newName: "ProcessStepExecution");

            migrationBuilder.RenameTable(
                name: "Process_Execution",
                newName: "ProcessExecution");

            migrationBuilder.RenameIndex(
                name: "IX_Process_Step_Execution_ProcessExecutionId",
                table: "ProcessStepExecution",
                newName: "IX_ProcessStepExecution_ProcessExecutionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProcessStepExecution",
                table: "ProcessStepExecution",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProcessExecution",
                table: "ProcessExecution",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessStepExecution_ProcessExecution_ProcessExecutionId",
                table: "ProcessStepExecution",
                column: "ProcessExecutionId",
                principalTable: "ProcessExecution",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
