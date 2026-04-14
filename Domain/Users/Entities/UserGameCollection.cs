using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
#pragma warning disable CS8618
    [Table("User_Game_Collections")]
    [Index(nameof(UserId), nameof(NormalizedName), IsUnique = true)]
    public class UserGameCollection
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(64)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(64)]
        public required string NormalizedName { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId)) ]
        public User User { get; set; }

        public ICollection<UserGameCollectionItem> Items { get; set; } = new List<UserGameCollectionItem>();
    }
}
