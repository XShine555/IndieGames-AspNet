using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Configurations
{
    public class StripeSettings
    {
        public const string SectionName = "Stripe";

        [Required]
        public required string SecretKey { get; set; }

        [Required]
        public required string WebhookSecret { get; set; }

        [Required]
        public required string PublishableKey { get; set; }

        public string Currency { get; set; } = "eur";
    }
}
