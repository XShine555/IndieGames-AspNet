using Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Genres")]
    public class Genre : IUpdatableEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(32)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(32)]
        public required string NormalizedName { get; set; }

        public ICollection<Game> Games { get; set; } = new List<Game>();

        public ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
