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
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Discount { get; set; } = 0m;

        [Required]
        [MaxLength(36)]
        public required string OwnerId { get; set; }

        [Required]
        public bool IsPublic { get; set; }

        [Required]
        public bool IsPublished { get; set; }

        public ICollection<GameStorePictures> StorePictures { get; set; } = new List<GameStorePictures>();

        public ICollection<GameArtwork> Artworks { get; set; } = new List<GameArtwork>();

        public ICollection<Genre> Genres { get; set; } = new List<Genre>();

        public ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();

        public ICollection<UserOwnedGame> UserOwnedGames { get; set; } = new List<UserOwnedGame>();

        [Required]
        public GameStoreReadinessStatus StoreReadinessStatus { get; set; } = GameStoreReadinessStatus.NotReadyForStore;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(OwnerId)) ]
        public User Owner { get; set; }
    }
}
