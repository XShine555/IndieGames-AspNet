using Application.Abstractions.Common;
using Application.Achievements.Responses;
using Domain.Entities;

namespace Application.Achievements.Mappers
{
    public class AchievementMapper : IAchievementMapper
    {
        public ApplicationAchievement ToApplicationAchievement(Achievement achievement)
        {
            return new ApplicationAchievement(
                achievement.Id,
                achievement.GameId,
                achievement.Name,
                achievement.Description,
                achievement.CreatedAt,
                achievement.UpdatedAt);
        }

        public ApplicationUserAchievement ToApplicationUserAchievement(Achievement achievement, UserAchievement? userAchievement)
        {
            return new ApplicationUserAchievement(
                achievement.GameId,
                achievement.Id,
                achievement.Name,
                achievement.Description,
                userAchievement is not null,
                userAchievement?.UnlockedAt
            );
        }
    }
}
