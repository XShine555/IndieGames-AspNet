using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Configurations
{
    public class S3Configuration
    {
        public const string SectionName = "S3";

        [Required]
        public required string Region { get; set; }

        [Required]
        public required string BucketName { get; set; }

        [Required]
        public required string AccessKey { get; set; }

        [Required]
        public required string SecretKey { get; set; }

        public string SessionToken { get; set; } = string.Empty;
    }
}