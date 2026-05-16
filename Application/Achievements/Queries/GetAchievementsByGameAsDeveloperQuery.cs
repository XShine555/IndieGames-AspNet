using Ardalis.Result;
using Application.Achievements.Responses;
using Mediator;

namespace Application.Achievements.Queries
{
    public record GetAchievementsByGameAsDeveloperQuery(
        Guid GameId,
        Guid UserId)
        : IQuery<Result<IReadOnlyList<ApplicationAchievement>>>;
}