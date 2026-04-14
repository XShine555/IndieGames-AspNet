using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
#pragma warning disable CS8618
    [Table("User_Owned_Games")]
    [PrimaryKey(nameof(UserId), nameof(GameId))]
    public class UserOwnedGame
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid GameId { get; set; }

        public User User { get; set; }

        public Game Game { get; set; }

        public DateTime purchasedAt { get; set; } = DateTime.UtcNow;
    }
}
