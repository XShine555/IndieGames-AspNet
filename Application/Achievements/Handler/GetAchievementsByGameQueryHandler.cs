using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Achievements.Queries;
using Application.Achievements.Responses;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList.EF;
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
            var baseQuery = database.Achievements
                .AsNoTracking()
                .Where(a => a.GameId == query.GameId)
                .OrderBy(a => a.Name)
                .Select(achievementMapper.ToApplicationAchievement)
                .AsQueryable();

            var totalCount = await baseQuery.CountAsync(cancellationToken);
            var achievements = await baseQuery.ToPagedListAsync(query.PageNumber, query.PageSize, totalCount, cancellationToken);

            return PaginatedApplicationResponse<ApplicationAchievement>.FromPagedList(achievements);
        }
    }
}
