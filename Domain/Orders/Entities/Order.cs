using Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
#pragma warning disable CS8618
    [Table("Orders")]
    public class Order : IUpdatableEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [MaxLength(128)]
        public string? StripeCheckoutSessionId { get; set; }

        [MaxLength(128)]
        public string? StripePaymentIntentId { get; set; }

        [Required]
        [MaxLength(8)]
        public string Currency { get; set; } = "usd";

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Column(TypeName = "decimal(18,2)")]
        public decimal RefundedAmount { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId)) ]
        public User User { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        public void AddItem(Guid gameId, decimal unitPrice, int quantity = 1)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity));
            }

            Items.Add(new OrderItem
            {
                OrderId = Id,
                GameId = gameId,
                UnitPrice = unitPrice,
                Quantity = quantity
            } );

            Status = OrderStatus.Pending;
        }

        public void RegisterCheckoutSession(string checkoutSessionId, string? paymentIntentId)
        {
            StripeCheckoutSessionId = checkoutSessionId;
            StripePaymentIntentId = paymentIntentId;
        }

        public bool MarkItemCompleted(Guid orderItemId)
        {
            var item = Items.SingleOrDefault(currentItem => currentItem.Id == orderItemId);
            if (item is null)
                return false;

            item.Status = OrderItemStatus.Completed;
            item.FailureReason = null;
            item.ProcessedAt = DateTime.UtcNow;
            RecalculateStatus();
            return true;
        }

        public bool MarkItemFailed(Guid orderItemId, string reason)
        {
            var item = Items.SingleOrDefault(currentItem => currentItem.Id == orderItemId);
            if (item is null)
                return false;

            item.Status = OrderItemStatus.Failed;
            item.FailureReason = reason;
            item.ProcessedAt = DateTime.UtcNow;
            RecalculateStatus();
            return true;
        }

        public decimal CalculateFailedAmount()
        {
            return Items
                .Where(item => item.Status == OrderItemStatus.Failed)
                .Sum(item => item.GetTotalPrice());
        }

        public decimal CalculateTotalAmount()
        {
            return Items.Sum(item => item.GetTotalPrice());
        }

        public void MarkRefunded(decimal amount)
        {
            if (amount <= 0m)
                return;

            RefundedAmount += amount;
        }

        private void RecalculateStatus()
        {
            if (Items.Count == 0)
            {
                Status = OrderStatus.Pending;
                return;
            }

            var completedItems = Items.Count(item => item.Status == OrderItemStatus.Completed);
            var failedItems = Items.Count(item => item.Status == OrderItemStatus.Failed);

            if (completedItems == Items.Count)
            {
                Status = OrderStatus.Completed;
                return;
            }

            if (failedItems == Items.Count)
            {
                Status = OrderStatus.Failed;
                return;
            }

            if (failedItems > 0)
            {
                Status = OrderStatus.PartiallyFailed;
                return;
            }

            Status = OrderStatus.Pending;
        }
    }
}
