using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Configurations
{
    public class MassTransitConfiguration
    {
        public const string SectionName = "MassTransit";

        [Required]
        public required string Address { get; set; }

        [Required]
        public required string AccessKey { get; set; }

        [Required]
        public required string SecretKey { get; set; }

        public string SessionToken { get; set; } = string.Empty;
    }
}