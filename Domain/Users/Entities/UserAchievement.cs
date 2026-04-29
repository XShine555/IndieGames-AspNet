using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
#pragma warning disable CS8618
    [Table("User_Achievements")]
    [PrimaryKey(nameof(UserId), nameof(AchievementId)) ]
    public class UserAchievement
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid AchievementId { get; set; }

        [Required]
        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId)) ]
        public User User { get; set; }

        [ForeignKey(nameof(AchievementId)) ]
        public Achievements Achievement { get; set; }
    }
}
