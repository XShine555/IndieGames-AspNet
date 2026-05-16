using Domain.Contracts;
using Domain.Users.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
#pragma warning disable CS8618
    [Table("Users")]
    public class User : IUpdatableEntity
    {
        [Key]
        public Guid IdentityId { get; set; }

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
        public UserRole Role { get; set; } = UserRole.User;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public UserProfilePicture ProfilePicture { get; set; }

        public ICollection<Game> CreatedGames { get; set; } = new List<Game>();

        public ICollection<UserOwnedGame> OwnedGames { get; set; } = new List<UserOwnedGame>();

        public ICollection<UserGameCollection> GamesCollections { get; set; } = new List<UserGameCollection>();

        public ICollection<UserCartItem> CartItems { get; set; } = new List<UserCartItem>();

        public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
    }
}
