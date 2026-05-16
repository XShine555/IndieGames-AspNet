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
            var achievements = await database.Achievements
                .AsNoTracking()
                .Where(a => a.GameId == query.GameId && a.IsPublished)
                .Include(a => a.AchievementPicture)
                .ToListAsync(cancellationToken);

            var userAchievements = await database.UserAchievements
                .AsNoTracking()
                .Where(ua => ua.UserId == query.UserId && achievements.Select(a => a.Id).Contains(ua.AchievementId))
                .ToListAsync(cancellationToken);

            var userAchievementsMap = userAchievements.ToDictionary(ua => ua.AchievementId);

            return achievements
                .Select(a =>
                {
                    userAchievementsMap.TryGetValue(a.Id, out var userAchievement);
                    return achievementMapper.ToApplicationUserAchievement(a, userAchievement);
                })
                .ToList();
        }
    }
}