using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
#pragma warning disable CS8618
    [Table("Game_Store_Pictures")]
    public class GameStorePictures
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int GameId { get; set; }

        [Required]
        [MaxLength(128)]
        public required string OriginalName { get; set; }

        [Required]
        [MaxLength(128)]
        public required string RelativePath { get; set; }

        [Required]
        [MaxLength(36)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(16)]
        public required string FileExtension { get; set; }

        [MaxLength(128)]
        public string? SmallRelativePath { get; set; }

        [MaxLength(36)]
        public string? SmallName { get; set; }

        [MaxLength(16)]
        public string? SmallFileExtension { get; set; }

        [MaxLength(128)]
        public string? MediumRelativePath { get; set; }

        [MaxLength(36)]
        public string? MediumName { get; set; }

        [MaxLength(16)]
        public string? MediumFileExtension { get; set; }

        [MaxLength(128)]
        public string? LargeRelativePath { get; set; }

        [MaxLength(36)]
        public string? LargeName { get; set; }

        [MaxLength(16)]
        public string? LargeFileExtension { get; set; }

        [Required]
        public GamePictureProcessingStatus ProcessingStatus { get; set; } = GamePictureProcessingStatus.Pending;

        [ForeignKey(nameof(GameId))]
        public Game Game { get; set; }

        [Required]
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
