using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdersAndStripePayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:game_artwork_processing_status", "pending,processing,completed,failed")
                .Annotation("Npgsql:Enum:game_artwork_type", "capsule,header,main")
                .Annotation("Npgsql:Enum:game_picture_processing_status", "pending,processing,completed,failed")
                .Annotation("Npgsql:Enum:game_store_readiness_status", "not_ready_for_store,ready_for_store")
                .Annotation("Npgsql:Enum:job_tracking_status", "running,succeeded,failed,compensated")
                .Annotation("Npgsql:Enum:job_tracking_type", "consumer,activity")
                .Annotation("Npgsql:Enum:order_item_status", "pending,completed,failed")
                .Annotation("Npgsql:Enum:order_status", "pending,completed,partially_failed,failed")
                .Annotation("Npgsql:Enum:stripe_event_processing_status", "processing,completed,failed")
                .OldAnnotation("Npgsql:Enum:game_artwork_processing_status", "pending,processing,completed,failed")
                .OldAnnotation("Npgsql:Enum:game_artwork_type", "capsule,header,main")
                .OldAnnotation("Npgsql:Enum:game_picture_processing_status", "pending,processing,completed,failed")
                .OldAnnotation("Npgsql:Enum:game_store_readiness_status", "not_ready_for_store,ready_for_store")
                .OldAnnotation("Npgsql:Enum:job_tracking_status", "running,succeeded,failed,compensated")
                .OldAnnotation("Npgsql:Enum:job_tracking_type", "consumer,activity");

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StripeCheckoutSessionId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    StripePaymentIntentId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Currency = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false, defaultValue: "usd"),
                    Status = table.Column<OrderStatus>(type: "order_status", nullable: false),
                    RefundedAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "IdentityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stripe_Event_Processing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    EventType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Status = table.Column<StripeEventProcessingStatus>(type: "stripe_event_processing_status", nullable: false),
                    Attempts = table.Column<int>(type: "integer", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastError = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stripe_Event_Processing", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Order_Items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<OrderItemStatus>(type: "order_item_status", nullable: false),
                    FailureReason = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Order_Items_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_Items_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_Items_GameId",
                table: "Order_Items",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Items_OrderId",
                table: "Order_Items",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_StripeCheckoutSessionId",
                table: "Orders",
                column: "StripeCheckoutSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_StripePaymentIntentId",
                table: "Orders",
                column: "StripePaymentIntentId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Stripe_Event_Processing_EventId",
                table: "Stripe_Event_Processing",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stripe_Event_Processing_Status",
                table: "Stripe_Event_Processing",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Order_Items");

            migrationBuilder.DropTable(
                name: "Stripe_Event_Processing");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:game_artwork_processing_status", "pending,processing,completed,failed")
                .Annotation("Npgsql:Enum:game_artwork_type", "capsule,header,main")
                .Annotation("Npgsql:Enum:game_picture_processing_status", "pending,processing,completed,failed")
                .Annotation("Npgsql:Enum:game_store_readiness_status", "not_ready_for_store,ready_for_store")
                .Annotation("Npgsql:Enum:job_tracking_status", "running,succeeded,failed,compensated")
                .Annotation("Npgsql:Enum:job_tracking_type", "consumer,activity")
                .OldAnnotation("Npgsql:Enum:game_artwork_processing_status", "pending,processing,completed,failed")
                .OldAnnotation("Npgsql:Enum:game_artwork_type", "capsule,header,main")
                .OldAnnotation("Npgsql:Enum:game_picture_processing_status", "pending,processing,completed,failed")
                .OldAnnotation("Npgsql:Enum:game_store_readiness_status", "not_ready_for_store,ready_for_store")
                .OldAnnotation("Npgsql:Enum:job_tracking_status", "running,succeeded,failed,compensated")
                .OldAnnotation("Npgsql:Enum:job_tracking_type", "consumer,activity")
                .OldAnnotation("Npgsql:Enum:order_item_status", "pending,completed,failed")
                .OldAnnotation("Npgsql:Enum:order_status", "pending,completed,partially_failed,failed")
                .OldAnnotation("Npgsql:Enum:stripe_event_processing_status", "processing,completed,failed");
        }
    }
}
