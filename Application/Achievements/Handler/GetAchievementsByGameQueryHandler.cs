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
                .ToListAsync(cancellationToken);

            HashSet<Guid> unlockedIds = [];
            if (query.UserId.HasValue)
            {
                var achievementIds = achievements.Select(a => a.Id).ToList();
                unlockedIds = (await database.UserAchievements
                    .AsNoTracking()
                    .Where(ua => ua.UserId == query.UserId.Value && achievementIds.Contains(ua.AchievementId))
                    .Select(ua => ua.AchievementId)
                    .ToListAsync(cancellationToken))
                    .ToHashSet();
            }

            var mapped = achievements
                .Select(a => achievementMapper.ToApplicationAchievement(a, unlockedIds.Contains(a.Id)))
                .ToList();

            var pagedList = mapped.ToPagedList(query.PageNumber, query.PageSize, mapped.Count);
            return PaginatedApplicationResponse<ApplicationAchievement>.FromPagedList(pagedList);
        }
    }
}
