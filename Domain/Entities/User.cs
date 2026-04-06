using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public required string IdentityId { get; set; }

        public ICollection<UserToGame> UserToGames { get; set; } = new List<UserToGame>();

        public ICollection<UserToGameRequest> UserToGameRequests { get; set; } = new List<UserToGameRequest>();

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public ICollection<Game> Games => UserToGames.Select(utg => utg.Game).ToList();

        [NotMapped]
        public ICollection<Game> RequestedGames => UserToGameRequests.Select(utgr => utgr.Game).ToList();
    }
}