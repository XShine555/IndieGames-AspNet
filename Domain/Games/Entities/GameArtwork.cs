using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
#pragma warning disable CS8618
    [Table("Game_Artworks")]
    [Index(nameof(GameId), nameof(Type), nameof(SortOrder), IsUnique = true)]
    [Index(nameof(GameId), nameof(Type)) ]
    public class GameArtwork
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid GameId { get; set; }

        [Required]
        public GameArtworkType Type { get; set; }

        [Required]
        public int SortOrder { get; set; }

        [Required]
        [MaxLength(256)]
        public required string OriginalRelativePath { get; set; }

        [Required]
        [MaxLength(128)]
        public required string OriginalFileName { get; set; }

        [Required]
        [MaxLength(16)]
        public required string OriginalExtension { get; set; }

        [Required]
        [MaxLength(256)]
        public string SmallRelativePath { get; set; } = string.Empty;

        [Required]
        [MaxLength(128)]
        public string SmallFileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(32)]
        public string SmallContentType { get; set; } = string.Empty;

        [Required]
        public int SmallWidth { get; set; }

        [Required]
        public int SmallHeight { get; set; }

        [Required]
        public long SmallFileSizeInBytes { get; set; }

        [Required]
        [MaxLength(256)]
        public string MediumRelativePath { get; set; } = string.Empty;

        [Required]
        [MaxLength(128)]
        public string MediumFileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(32)]
        public string MediumContentType { get; set; } = string.Empty;

        [Required]
        public int MediumWidth { get; set; }

        [Required]
        public int MediumHeight { get; set; }

        [Required]
        public long MediumFileSizeInBytes { get; set; }

        [Required]
        [MaxLength(256)]
        public string LargeRelativePath { get; set; } = string.Empty;

        [Required]
        [MaxLength(128)]
        public string LargeFileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(32)]
        public string LargeContentType { get; set; } = string.Empty;

        [Required]
        public int LargeWidth { get; set; }

        [Required]
        public int LargeHeight { get; set; }

        [Required]
        public long LargeFileSizeInBytes { get; set; }

        [Required]
        public GameArtworkProcessingStatus ProcessingStatus { get; set; } = GameArtworkProcessingStatus.Pending;

        [Required]
        [MaxLength(512)]
        public string ProcessingError { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(GameId)) ]
        public Game Game { get; set; }
    }
}
