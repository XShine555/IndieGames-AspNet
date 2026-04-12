using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
#pragma warning disable CS8618
    [Table("Game_Artwork_Variants")]
    [Index(nameof(GameArtworkId), nameof(Size), nameof(PictureContentType), IsUnique = true)]
    [Index(nameof(GameArtworkId), nameof(IsPrimary)) ]
    public class GameArtworkVariant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int GameArtworkId { get; set; }

        [Required]
        public GameArtworkVariantSize Size { get; set; }

        [Required]
        [MaxLength(128)]
        public required string PictureContentType { get; set; }

        [Required]
        [MaxLength(256)]
        public required string RelativePath { get; set; }

        [Required]
        [MaxLength(128)]
        public required string FileName { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Width { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Height { get; set; }

        [Required]
        [Range(1, long.MaxValue)]
        public long FileSizeInBytes { get; set; }

        [Required]
        public bool IsPrimary { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(GameArtworkId)) ]
        public GameArtwork GameArtwork { get; set; }
    }
}
