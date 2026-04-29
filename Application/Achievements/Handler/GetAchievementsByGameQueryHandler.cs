using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Achievements.Queries;
using Application.Achievements.Responses;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace Application.Achievements.Handler
{
    public class GetAchievementsByGameQueryHandler(IDatabase database, IAchievementMapper achievementMapper)
        : IQueryHandler<GetAchievementsByGameQuery, PaginatedApplicationResponse<ApplicationAchievement>>
    {
        public async ValueTask<PaginatedApplicationResponse<ApplicationAchievement>> Handle(
            GetAchievementsByGameQuery query,
            CancellationToken cancellationToken)
        {
            var achievements = await database.Achievements
                .AsNoTracking()
                .Where(a => a.GameId == query.GameId)
                .OrderBy(a => a.Name)
                .Select(achievementMapper.ToApplicationAchievementFunction)
                .ToListAsync(cancellationToken);

            var totalCount = achievements.Count;
            var pagedList = achievements.ToPagedList(query.PageNumber, query.PageSize, totalCount);
            return PaginatedApplicationResponse<ApplicationAchievement>.FromPagedList(pagedList);
        }
    }
}
