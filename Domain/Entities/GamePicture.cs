using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
#pragma warning disable CS8618
    [Table("game_pictures")]
    public class GamePicture
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int GameId { get; set; }

        [Required]
        [MaxLength(128)]
        public required string PictureKey { get; set; }

        [ForeignKey(nameof(GameId) )]
        public Game Game { get; set; }

        [Required]
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}