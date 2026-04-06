using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
#pragma warning disable CS8618
    [Table("Games")]
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

        public ICollection<Genre> Genres { get; set; } = new List<Genre>();

        public ICollection<UserToGame> UsersToGames { get; set; } = new List<UserToGame>();

        public ICollection<UserToGameRequest> UsersToGameRequests { get; set; } = new List<UserToGameRequest>();

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public ICollection<User> Users => UsersToGames.Select(utg => utg.User).ToList();

        [NotMapped]
        public ICollection<User> RequestedUsers => UsersToGameRequests.Select(utgr => utgr.User).ToList();
    }
}