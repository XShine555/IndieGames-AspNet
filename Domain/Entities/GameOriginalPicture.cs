using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
#pragma warning disable CS8618
    [Table("Game_Original_Pictures")]
    public class GameOriginalPicture
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

        [ForeignKey(nameof(GameId) )]
        public Game Game { get; set; }

        [Required]
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}