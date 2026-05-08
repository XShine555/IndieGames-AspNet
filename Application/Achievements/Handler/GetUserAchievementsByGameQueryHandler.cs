using Application.Achievements.Queries;
using Application.Achievements.Responses;
using Application.Abstractions.Persistence;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions.Common;
using Domain.Entities;

namespace Application.Achievements.Handler
{
    public class GetUserAchievementsByGameQueryHandler(IDatabase database, IAchievementMapper achievementMapper)
        : IQueryHandler<GetUserAchievementsByGameQuery, IReadOnlyList<ApplicationUserAchievement>>
    {
        public async ValueTask<IReadOnlyList<ApplicationUserAchievement>> Handle(GetUserAchievementsByGameQuery query, CancellationToken cancellationToken)
        {
            var items = await database.Achievements
                .AsNoTracking()
                .Where(a => a.GameId == query.GameId && a.IsPublished)
                .Include(a => a.AchievementPicture)
                .GroupJoin(
                    database.UserAchievements.Where(ua => ua.UserId == query.UserId),
                    a => a.Id,
                    ua => ua.AchievementId,
                    (achievement, userAchievements) => new { Achievement = achievement, UserAchievement = userAchievements.SingleOrDefault() } )
                .Select(x => achievementMapper.ToApplicationUserAchievement(x.Achievement, x.UserAchievement))
                .ToListAsync(cancellationToken);

            return items;
        }
    }
}