using Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Users")]
    public class User : IUpdatableEntity
    {
        [Key]
        [MaxLength(36)]
        public required string IdentityId { get; set; }

        [Required]
        [MaxLength(24)]
        public required string Username { get; set; }

        [Required]
        [MaxLength(24)]
        public required string DisplayUsername { get; set; }

        [Required]
        [MaxLength(24)]
        public required string NormalizedDisplayUsername { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public ICollection<Game> CreatedGames { get; set; } = new List<Game>();

        public ICollection<UserOwnedGame> OwnedGames { get; set; } = new List<UserOwnedGame>();
    }
}