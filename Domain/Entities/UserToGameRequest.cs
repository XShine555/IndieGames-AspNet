using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
#pragma warning disable CS8618
    [Table("Users_To_Games_Requests")]
    [PrimaryKey(nameof(UserId), nameof(GameId)) ]
    public class UserToGameRequest
    {
        public long UserId { get; set; }

        public long GameId { get; set; }

        [ForeignKey(nameof(UserId)) ]
        public User User { get; set; }

        [ForeignKey(nameof(GameId)) ]
        public Game Game { get; set; }

        [Required]
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }
}