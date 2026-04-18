using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
#pragma warning disable CS8618
    [Table("Stripe_Event_Processing")]
    public class StripeEventProcessing
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(128)]
        public required string EventId { get; set; }

        [Required]
        [MaxLength(128)]
        public required string EventType { get; set; }

        [Required]
        public StripeEventProcessingStatus Status { get; set; } = StripeEventProcessingStatus.Processing;

        [Required]
        public int Attempts { get; set; } = 1;

        [Required]
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ProcessedAt { get; set; }

        [MaxLength(2048)]
        public string? LastError { get; set; }
    }
}
