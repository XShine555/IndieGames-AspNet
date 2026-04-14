using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
#pragma warning disable CS8618
    [Table("Game_Store_Pictures")]
    public class GameStorePictures
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid GameId { get; set; }

        [Required]
        [MaxLength(128)]
        public required string OriginalName { get; set; }

        [Required]
        [MaxLength(128)]
        public required string OriginalRelativePath { get; set; }

        [Required]
        [MaxLength(32)]
        public required string OriginalContentType { get; set; }

        [MaxLength(128)]
        public string? SmallRelativePath { get; set; }

        [MaxLength(36)]
        public string? SmallName { get; set; }

        [MaxLength(16)]
        public string? SmallFileContentType { get; set; }

        [MaxLength(128)]
        public string? MediumRelativePath { get; set; }

        [MaxLength(36)]
        public string? MediumName { get; set; }

        [MaxLength(16)]
        public string? MediumFileContentType { get; set; }

        [MaxLength(128)]
        public string? LargeRelativePath { get; set; }

        [MaxLength(36)]
        public string? LargeName { get; set; }

        [MaxLength(16)]
        public string? LargeContentType { get; set; }

        [Required]
        public GamePictureProcessingStatus ProcessingStatus { get; set; } = GamePictureProcessingStatus.Pending;

        [ForeignKey(nameof(GameId))]
        public Game Game { get; set; }

        [Required]
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
