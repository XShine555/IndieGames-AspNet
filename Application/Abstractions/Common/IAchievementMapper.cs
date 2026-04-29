using Application.Achievements.Responses;
using AchievementEntity = Domain.Entities.Achievements;
using System.Linq.Expressions;

namespace Application.Abstractions.Common
{
    public interface IAchievementMapper
    {
        ApplicationAchievement ToApplicationAchievement(AchievementEntity achievement);

        Expression<Func<AchievementEntity, ApplicationAchievement>> ToApplicationAchievementFunction { get; }
    }
}
