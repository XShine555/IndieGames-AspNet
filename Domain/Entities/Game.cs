using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
#pragma warning disable CS8618
    [Table("CreatedGames")]
    public class Game
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public required string Title { get; set; }

        [Required]
        public required string NormalizedTitle { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        public required Guid OwnerId { get; set; }

        public ICollection<Genre> Genres { get; set; } = new List<Genre>();

        public ICollection<UserOwnedGame> UserOwnedGames { get; set; } = new List<UserOwnedGame>();

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(OwnerId)) ]
        public User Owner { get; set; }
    }
}