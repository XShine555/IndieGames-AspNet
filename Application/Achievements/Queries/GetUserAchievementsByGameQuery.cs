using Application.Achievements.Responses;
using Mediator;

namespace Application.Achievements.Queries
{
    public record GetUserAchievementsByGameQuery(
        Guid UserId,
        Guid GameId)
        : IQuery<IReadOnlyList<ApplicationUserAchievement>>;
}