using Application.Achievements.Responses;
using Domain.Entities;

namespace Application.Abstractions.Common
{
    public interface IAchievementMapper
    {
        ApplicationAchievement ToApplicationAchievement(Achievement achievement);

        ApplicationUserAchievement ToApplicationUserAchievement(Achievement achievement, UserAchievement? userAchievement);
    }
}
