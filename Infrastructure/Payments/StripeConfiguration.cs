using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Payments
{
    public class StripeConfiguration
    {
        public const string SectionName = "Stripe";

        [Required]
        public required string SecretKey { get; set; }

        [Required]
        public required string WebhookSecret { get; set; }

        [Required]
        [MaxLength(8)]
        public required string DefaultCurrency { get; set; }
    }
}
