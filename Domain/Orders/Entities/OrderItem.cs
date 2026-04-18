using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
#pragma warning disable CS8618
    [Table("Order_Items")]
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public Guid GameId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        public int Quantity { get; set; } = 1;

        [Required]
        public OrderItemStatus Status { get; set; } = OrderItemStatus.Pending;

        [MaxLength(512)]
        public string? FailureReason { get; set; }

        public DateTime? ProcessedAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }

        [ForeignKey(nameof(GameId))]
        public Game Game { get; set; }

        public decimal GetTotalPrice() => UnitPrice * Quantity;
    }
}
