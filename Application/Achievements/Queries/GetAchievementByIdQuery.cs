using Application.Achievements.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Achievements.Queries
{
    public record GetAchievementByIdQuery(Guid AchievementId)
        : IQuery<Result<ApplicationAchievement>>;
}
