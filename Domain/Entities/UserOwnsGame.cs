using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
#pragma warning disable CS8618
    [Table("Users_owned_games")]
    [PrimaryKey(nameof(UserId), nameof(GameId)) ]
    public class UserOwnsGame
    {
        [Required]
        public required Guid UserId { get; set; }

        [Required]
        public required Guid GameId { get; set; }

        public User User { get; set; }

        public Game Game { get; set; }

        public DateTime purchasedAt { get; set; } = DateTime.UtcNow;
    }
}