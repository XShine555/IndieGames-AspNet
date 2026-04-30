using Application.Abstractions.Common;
using Application.Achievements.Responses;
using Mediator;

namespace Application.Achievements.Queries
{
    public record GetAchievementsByGameQuery(
        Guid GameId,
        int PageNumber = 1,
        int PageSize = 10)
        : IQuery<PaginatedApplicationResponse<ApplicationAchievement>>;
}
