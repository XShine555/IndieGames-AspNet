using Domain.Games.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
#pragma warning disable CS8618
    [Table("Achievement_Pictures")]
    public class AchievementPicture
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public required Guid AchievementId { get; set; }

        [Required]
        [MaxLength(128)]
        public required string OriginalName { get; set; }

        [Required]
        [MaxLength(128)]
        public required string? OriginalRelativePath { get; set; }

        [Required]
        [MaxLength(32)]
        public required string? OriginalContentType { get; set; }

        [MaxLength(128)]
        public string? SmallRelativePath { get; set; }

        [MaxLength(48)]
        public string? SmallName { get; set; }

        [MaxLength(32)]
        public string? SmallFileContentType { get; set; }

        [MaxLength(128)]
        public string? MediumRelativePath { get; set; }

        [MaxLength(48)]
        public string? MediumName { get; set; }

        [MaxLength(32)]
        public string? MediumFileContentType { get; set; }

        [MaxLength(128)]
        public string? LargeRelativePath { get; set; }

        [MaxLength(48)]
        public string? LargeName { get; set; }

        [MaxLength(32)]
        public string? LargeContentType { get; set; }

        [Required]
        public AchievementPictureProcessingStatus ProcessingStatus { get; set; } = AchievementPictureProcessingStatus.Pending;

        [Required]
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(AchievementId)) ]
        public Achievement Achievement { get; set; }
    }
}
