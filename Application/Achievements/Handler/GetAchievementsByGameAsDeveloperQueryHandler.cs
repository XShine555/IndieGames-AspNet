using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Achievements.Queries;
using Application.Achievements.Responses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Achievements.Handler
{
    public class GetAchievementsByGameAsDeveloperQueryHandler(
        IDatabase database,
        ILogger<GetAchievementsByGameAsDeveloperQueryHandler> logger,
        IAchievementMapper achievementMapper)
        : IQueryHandler<GetAchievementsByGameAsDeveloperQuery, Result<IReadOnlyList<ApplicationAchievement>>>
    {
        public async ValueTask<Result<IReadOnlyList<ApplicationAchievement> >> Handle(GetAchievementsByGameAsDeveloperQuery query, CancellationToken cancellationToken)
        {
            var game = await database.Games
                .AsNoTracking()
                .SingleOrDefaultAsync(g => g.Id == query.GameId, cancellationToken);

            if (game is null)
            {
                logger.LogWarning("Game with id {GameId} not found while fetching achievements as developer", query.GameId);
                return Result.NotFound();
            }

            if (game.OwnerId != query.UserId)
            {
                logger.LogWarning("User {UserId} attempted to fetch developer achievements for game {GameId} without owning it", query.UserId, query.GameId);
                return Result.Unauthorized();
            }

            var achievements = await database.Achievements
                .AsNoTracking()
                .Where(a => a.GameId == query.GameId)
                .OrderBy(a => a.Name)
                .Include(a => a.AchievementPicture)
                .ToListAsync(cancellationToken);

            var result = new List<ApplicationAchievement>();
            foreach (var a in achievements)
            {
                result.Add(achievementMapper.ToApplicationAchievement(a));
            }

            return Result.Success<IReadOnlyList<ApplicationAchievement>>(result);
        }
    }
}
