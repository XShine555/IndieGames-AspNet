using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Messaging.Configuration
{
    public class MessagingConfiguration
    {
        public const string SectionName = "Messaging";

        [Required]
        public required string Address { get; set; }

        [Required]
        public required string AccessKey { get; set; }

        [Required]
        public required string SecretKey { get; set; }

        public string SessionToken { get; set; } = string.Empty;
    }
}
