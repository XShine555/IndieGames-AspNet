using Application.Abstractions.Common;
using Application.Achievements.Responses;
using AchievementEntity = Domain.Entities.Achievements;
using System.Linq.Expressions;

namespace Application.Achievements.Mappers
{
    public class AchievementMapper : IAchievementMapper
    {
        public ApplicationAchievement ToApplicationAchievement(AchievementEntity achievement, bool isUnlocked = false)
        {
            return new ApplicationAchievement(
                achievement.Id,
                achievement.GameId,
                achievement.Name,
                achievement.Description,
                isUnlocked,
                achievement.CreatedAt,
                achievement.UpdatedAt);
        }

        public Expression<Func<AchievementEntity, ApplicationAchievement>> ToApplicationAchievementFunction => achievement => new ApplicationAchievement(
            achievement.Id,
            achievement.GameId,
            achievement.Name,
            achievement.Description,
            false,
            achievement.CreatedAt,
            achievement.UpdatedAt);
    }
}
