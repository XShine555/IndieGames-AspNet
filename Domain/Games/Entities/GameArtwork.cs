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
        public int Id { get; set; }

        [Required]
        public int GameId { get; set; }

        [Required]
        public GameArtworkType Type { get; set; }

        [Required]
        public GameArtworkKind Kind { get; set; } = GameArtworkKind.Single;

        [Required]
        public int SortOrder { get; set; }

        [Required]
        [MaxLength(256)]
        public required string OriginalRelativePath { get; set; }

        [Required]
        [MaxLength(128)]
        public required string OriginalFileName { get; set; }

        [Required]
        public string OriginalExtension { get; set; }

        [Required]
        public GameArtworkProcessingStatus ProcessingStatus { get; set; } = GameArtworkProcessingStatus.Pending;

        [MaxLength(512)]
        public string? ProcessingError { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(GameId)) ]
        public Game Game { get; set; }

        public ICollection<GameArtworkVariant> Variants { get; set; } = new List<GameArtworkVariant>();

        public void AddVariant(GameArtworkVariant variant)
        {
            if (variant.IsPrimary && Variants.Any(v => v.IsPrimary))
                throw new InvalidOperationException("An artwork can only have one primary variant.");

            if (Variants.Any(v => v.Size == variant.Size && v.PictureContentType == variant.PictureContentType))
                throw new InvalidOperationException("Duplicate artwork variant for the same size and format.");

            Variants.Add(variant);
        }
    }
}
