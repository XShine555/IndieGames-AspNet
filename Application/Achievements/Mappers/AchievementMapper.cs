using Application.Abstractions.Common;
using Application.Achievements.Responses;
using Domain.Entities;
using System.Linq.Expressions;

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
                achievement.AchievementPicture.SmallRelativePath,
                achievement.AchievementPicture.MediumRelativePath,
                achievement.AchievementPicture.LargeRelativePath,
                achievement.AchievementPicture.ProcessingStatus,
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
                achievement.AchievementPicture.SmallRelativePath,
                achievement.AchievementPicture.MediumRelativePath,
                achievement.AchievementPicture.LargeRelativePath,
                userAchievement is not null,
                userAchievement != null ? userAchievement.UnlockedAt : null);
        }

        public Expression<Func<Achievement, ApplicationAchievement>> ToApplicationAchievementFunction => achievement => new ApplicationAchievement(
            achievement.Id,
            achievement.GameId,
            achievement.Name,
            achievement.Description,
            achievement.AchievementPicture.SmallRelativePath,
            achievement.AchievementPicture.MediumRelativePath,
            achievement.AchievementPicture.LargeRelativePath,
            achievement.AchievementPicture.ProcessingStatus,
            achievement.CreatedAt,
            achievement.UpdatedAt);
    }
}
