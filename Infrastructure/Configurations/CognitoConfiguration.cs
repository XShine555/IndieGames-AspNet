using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Configurations
{
    public class CognitoConfiguration
    {
        public const string SectionName = "Cognito";

        [Required]
        public required string Region { get; set; }

        [Required]
        public required string UserPoolId { get; set; }

        [Required]
        public required string AccessKey { get; set; }

        [Required]
        public required string SecretKey { get; set; }

        public string SessionToken { get; set; } = string.Empty;
    }
}