using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
#pragma warning disable CS8618
    [Table("User_Cart_Items")]
    [PrimaryKey(nameof(UserId), nameof(GameId))]
    public class UserCartItem
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid GameId { get; set; }

        [Required]
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [ForeignKey(nameof(GameId))]
        public Game Game { get; set; }
    }
}
