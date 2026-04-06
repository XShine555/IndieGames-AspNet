using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Configurations
{
    public class DatabaseConfiguration
    {
        public const string SectionName = "Database";

        [Required]
        public required string ConnectionString { get; set; }
    }
}