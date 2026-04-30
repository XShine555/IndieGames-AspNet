using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
#pragma warning disable CS8618
    [Table("Achievement_Pictures")]
    public class AchievementPicture
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid AchievementId { get; set; }

        [MaxLength(128)]
        public string? OriginalName { get; set; }

        [MaxLength(128)]
        public string? OriginalRelativePath { get; set; }

        [MaxLength(32)]
        public string? OriginalContentType { get; set; }

        [Required]
        [MaxLength(128)]
        public required string SmallRelativePath { get; set; }

        [Required]
        [MaxLength(48)]
        public required string SmallName { get; set; }

        [Required]
        [MaxLength(32)]
        public required string SmallFileContentType { get; set; }

        [Required]
        [MaxLength(128)]
        public required string MediumRelativePath { get; set; }

        [Required]
        [MaxLength(48)]
        public required string MediumName { get; set; }

        [Required]
        [MaxLength(32)]
        public required string MediumFileContentType { get; set; }

        [Required]
        [MaxLength(128)]
        public required string LargeRelativePath { get; set; }

        [Required]
        [MaxLength(48)]
        public required string LargeName { get; set; }

        [Required]
        [MaxLength(32)]
        public required string LargeContentType { get; set; }

        [Required]
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(AchievementId)) ]
        public Achievement Achievement { get; set; }
    }
}
