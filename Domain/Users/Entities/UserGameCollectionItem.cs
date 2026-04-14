using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
#pragma warning disable CS8618
    [Table("User_Game_Collection_Items")]
    [PrimaryKey(nameof(CollectionId), nameof(GameId))]
    public class UserGameCollectionItem
    {
        [Required]
        public Guid CollectionId { get; set; }

        [Required]
        public Guid GameId { get; set; }

        [Required]
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(CollectionId))]
        public UserGameCollection Collection { get; set; }

        [ForeignKey(nameof(GameId))]
        public Game Game { get; set; }
    }
}
