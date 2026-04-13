using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
#pragma warning disable CS8618
    [Table("Game_Genres")]
    [PrimaryKey(nameof(GameId), nameof(GenreId))]
    public class GameGenre
    {
        [Required]
        public int GameId { get; set; }

        [Required]
        public int GenreId { get; set; }

        [Required]
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public Game Game { get; set; }

        public Genre Genre { get; set; }
    }
}
