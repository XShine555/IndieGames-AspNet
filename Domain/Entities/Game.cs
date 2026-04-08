using Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
#pragma warning disable CS8618
    [Table("Games")]
    public class Game : IUpdatableEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public required string Title { get; set; }

        [Required]
        [MaxLength(64)]
        public required string NormalizedTitle { get; set; }

        [Required]
        [MaxLength(1024)]
        public required string Description { get; set; }

        [Required]
        [MaxLength(36)]
        public required string OwnerId { get; set; }

        public ICollection<GameOriginalPicture> Pictures { get; set; } = new List<GameOriginalPicture>();

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